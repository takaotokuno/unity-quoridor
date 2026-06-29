using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Quoridor
{
    /// <summary>
    /// 思考時間を上限に反復深化 αβ 探索を行い、時間到達時点で見つかっている最善手を返す Strategy。
    /// 外部から固定 depth を指定せず、制限時間内に完了した最深探索の結果を採用する。
    /// Coroutine とフレーム単位の探索予算で分割実行し、メインスレッドの長時間占有を避ける。
    /// </summary>
    public sealed class AlphaBetaCpuAgentStrategy : CpuSearchStrategyBase
    {
        private const int FrameSearchBudgetMilliseconds = 5;
        private const int WinScore = 100000;
        private const int MaxSearchDepthSafetyLimit = 64;
        private const int MinScoreSentinel = int.MinValue + 1;
        private const int MaxScoreSentinel = int.MaxValue;

        public AlphaBetaCpuAgentStrategy(
            LegalCommandEnumerator legalCommandEnumerator,
            CpuCommandSimulator commandSimulator,
            IRandomProvider randomProvider
        )
            : base(
                legalCommandEnumerator,
                commandSimulator,
                randomProvider
            )
        {
        }

        public override IEnumerator DecideCommand(
            CpuAgentDecisionContext context,
            Action<IMatchCommand> onDecided
        )
        {
            yield return null;

            var rootCandidates = EnumerateAllLegalCommands(context);
            if (rootCandidates.Count == 0)
            {
                onDecided?.Invoke(null);
                yield break;
            }

            var totalStopwatch = Stopwatch.StartNew();
            var frameStopwatch = Stopwatch.StartNew();
            TimeSpan timeLimit = TimeSpan.FromMilliseconds(context.SearchTimeLimit.Value);
            TimeSpan frameBudget = TimeSpan.FromMilliseconds(FrameSearchBudgetMilliseconds);
            IMatchCommand bestCommand = PickRandomCommand(rootCandidates);

            for (int depth = 1; depth <= MaxSearchDepthSafetyLimit; depth++)
            {
                if (IsTimeExpired(totalStopwatch, timeLimit))
                    break;

                SearchIterationResult result = SearchIterationResult.Timeout();
                yield return SearchRoot(
                    context,
                    rootCandidates,
                    depth,
                    totalStopwatch,
                    timeLimit,
                    frameStopwatch,
                    frameBudget,
                    value => result = value
                );

                if (!result.Completed)
                    break;

                bestCommand = result.Command;

                if (IsWinningScore(result.Score))
                    break;
            }

            onDecided?.Invoke(bestCommand);
        }

        private IEnumerator SearchRoot(
            CpuAgentDecisionContext context,
            IReadOnlyList<IMatchCommand> candidates,
            int depth,
            Stopwatch totalStopwatch,
            TimeSpan timeLimit,
            Stopwatch frameStopwatch,
            TimeSpan frameBudget,
            Action<SearchIterationResult> onCompleted
        )
        {
            CpuBestCommandAccumulator bestCommands = CreateBestCommandAccumulator();
            int alpha = MinScoreSentinel;
            const int beta = MaxScoreSentinel;

            foreach (IMatchCommand candidate in candidates)
            {
                if (IsTimeExpired(totalStopwatch, timeLimit))
                {
                    onCompleted?.Invoke(SearchIterationResult.Timeout());
                    yield break;
                }

                if (IsFrameBudgetExpired(frameStopwatch, frameBudget))
                {
                    yield return null;
                    frameStopwatch.Restart();
                }

                CpuCommandSimulationResult simulation = SimulateCompletedTurn(context.State, candidate);
                if (!simulation.Succeeded)
                    continue;

                int score = AlphaBeta(
                    context,
                    simulation.State,
                    context.PlayerId,
                    depth - 1,
                    alpha,
                    beta,
                    totalStopwatch,
                    timeLimit,
                    frameStopwatch,
                    frameBudget
                );

                if (IsTimeExpired(totalStopwatch, timeLimit))
                {
                    onCompleted?.Invoke(SearchIterationResult.Timeout());
                    yield break;
                }

                bestCommands.Add(candidate, score);
                alpha = Math.Max(alpha, bestCommands.BestScore);
            }

            CpuBestCommandSelection selection = bestCommands.Select();
            onCompleted?.Invoke(selection.HasCommand
                ? SearchIterationResult.Success(selection.Command, selection.Score)
                : SearchIterationResult.Timeout());
        }

        private int AlphaBeta(
            CpuAgentDecisionContext context,
            MatchState state,
            PlayerId perspectivePlayerId,
            int depthRemaining,
            int alpha,
            int beta,
            Stopwatch totalStopwatch,
            TimeSpan timeLimit,
            Stopwatch frameStopwatch,
            TimeSpan frameBudget
        )
        {
            if (ShouldEvaluateCurrentState(state, depthRemaining, totalStopwatch, timeLimit, frameStopwatch, frameBudget))
                return Evaluate(context, state, perspectivePlayerId);

            var candidates = EnumerateAllLegalCommands(CreateDecisionContext(
                state,
                context.SearchTimeLimit,
                context.Evaluator
            ));
            if (candidates.Count == 0)
                return Evaluate(context, state, perspectivePlayerId);

            return IsMaximizingTurn(state, perspectivePlayerId)
                ? SearchMaximizingNode(
                    context,
                    state,
                    perspectivePlayerId,
                    candidates,
                    depthRemaining,
                    alpha,
                    beta,
                    totalStopwatch,
                    timeLimit,
                    frameStopwatch,
                    frameBudget
                )
                : SearchMinimizingNode(
                    context,
                    state,
                    perspectivePlayerId,
                    candidates,
                    depthRemaining,
                    alpha,
                    beta,
                    totalStopwatch,
                    timeLimit,
                    frameStopwatch,
                    frameBudget
                );
        }

        private int SearchMaximizingNode(
            CpuAgentDecisionContext context,
            MatchState state,
            PlayerId perspectivePlayerId,
            IReadOnlyList<IMatchCommand> candidates,
            int depthRemaining,
            int alpha,
            int beta,
            Stopwatch totalStopwatch,
            TimeSpan timeLimit,
            Stopwatch frameStopwatch,
            TimeSpan frameBudget
        )
        {
            int value = MinScoreSentinel;

            foreach (IMatchCommand candidate in candidates)
            {
                if (IsTimeExpired(totalStopwatch, timeLimit))
                    return value;

                if (IsFrameBudgetExpired(frameStopwatch, frameBudget))
                    return Evaluate(context, state, perspectivePlayerId);

                if (!TrySearchChild(
                    context,
                    state,
                    perspectivePlayerId,
                    candidate,
                    depthRemaining,
                    alpha,
                    beta,
                    totalStopwatch,
                    timeLimit,
                    frameStopwatch,
                    frameBudget,
                    out int score
                ))
                    continue;

                value = Math.Max(value, score);
                alpha = Math.Max(alpha, value);

                if (alpha >= beta)
                    break;
            }

            return value == MinScoreSentinel
                ? Evaluate(context, state, perspectivePlayerId)
                : value;
        }

        private int SearchMinimizingNode(
            CpuAgentDecisionContext context,
            MatchState state,
            PlayerId perspectivePlayerId,
            IReadOnlyList<IMatchCommand> candidates,
            int depthRemaining,
            int alpha,
            int beta,
            Stopwatch totalStopwatch,
            TimeSpan timeLimit,
            Stopwatch frameStopwatch,
            TimeSpan frameBudget
        )
        {
            int value = MaxScoreSentinel;

            foreach (IMatchCommand candidate in candidates)
            {
                if (IsTimeExpired(totalStopwatch, timeLimit))
                    return value;

                if (IsFrameBudgetExpired(frameStopwatch, frameBudget))
                    return Evaluate(context, state, perspectivePlayerId);

                if (!TrySearchChild(
                    context,
                    state,
                    perspectivePlayerId,
                    candidate,
                    depthRemaining,
                    alpha,
                    beta,
                    totalStopwatch,
                    timeLimit,
                    frameStopwatch,
                    frameBudget,
                    out int score
                ))
                    continue;

                value = Math.Min(value, score);
                beta = Math.Min(beta, value);

                if (alpha >= beta)
                    break;
            }

            return value == MaxScoreSentinel
                ? Evaluate(context, state, perspectivePlayerId)
                : value;
        }

        private bool TrySearchChild(
            CpuAgentDecisionContext context,
            MatchState state,
            PlayerId perspectivePlayerId,
            IMatchCommand candidate,
            int depthRemaining,
            int alpha,
            int beta,
            Stopwatch totalStopwatch,
            TimeSpan timeLimit,
            Stopwatch frameStopwatch,
            TimeSpan frameBudget,
            out int score
        )
        {
            CpuCommandSimulationResult simulation = SimulateCompletedTurn(state, candidate);
            if (!simulation.Succeeded)
            {
                score = 0;
                return false;
            }

            score = AlphaBeta(
                context,
                simulation.State,
                perspectivePlayerId,
                depthRemaining - 1,
                alpha,
                beta,
                totalStopwatch,
                timeLimit,
                frameStopwatch,
                frameBudget
            );
            return true;
        }

        private static CpuAgentDecisionContext CreateDecisionContext(
            MatchState state,
            CpuSearchTimeLimit searchTimeLimit,
            ICpuEvaluator evaluator
        )
        {
            return new CpuAgentDecisionContext(
                state,
                state.CurrentPlayerId,
                MatchCommandIssuers.CpuAgent,
                searchTimeLimit,
                evaluator
            );
        }

        private static bool ShouldEvaluateCurrentState(
            MatchState state,
            int depthRemaining,
            Stopwatch totalStopwatch,
            TimeSpan timeLimit,
            Stopwatch frameStopwatch,
            TimeSpan frameBudget
        )
        {
            return depthRemaining <= 0
                || !state.IsInProgress
                || IsTimeExpired(totalStopwatch, timeLimit)
                || IsFrameBudgetExpired(frameStopwatch, frameBudget);
        }

        private static bool IsMaximizingTurn(MatchState state, PlayerId perspectivePlayerId)
        {
            return state.CurrentPlayerId == perspectivePlayerId;
        }

        private static bool IsWinningScore(int score)
        {
            return Math.Abs(score) >= WinScore;
        }

        private static bool IsFrameBudgetExpired(Stopwatch frameStopwatch, TimeSpan frameBudget)
        {
            return frameStopwatch.Elapsed >= frameBudget;
        }

        private static bool IsTimeExpired(Stopwatch stopwatch, TimeSpan timeLimit)
        {
            return stopwatch.Elapsed >= timeLimit;
        }

        private readonly struct SearchIterationResult
        {
            public bool Completed { get; }
            public IMatchCommand Command { get; }
            public int Score { get; }

            private SearchIterationResult(bool completed, IMatchCommand command, int score)
            {
                Completed = completed;
                Command = command;
                Score = score;
            }

            public static SearchIterationResult Success(IMatchCommand command, int score)
            {
                return new SearchIterationResult(true, command, score);
            }

            public static SearchIterationResult Timeout()
            {
                return new SearchIterationResult(false, null, 0);
            }
        }

    }
}
