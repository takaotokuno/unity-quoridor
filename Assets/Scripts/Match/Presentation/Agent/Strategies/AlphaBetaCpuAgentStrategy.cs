using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

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
        private readonly SearchProfiler _searchProfiler;
        private readonly Dictionary<SearchEvaluationCacheKey, int> _evaluationCache = new();

        public AlphaBetaCpuAgentStrategy(
            LegalCommandEnumerator legalCommandEnumerator,
            CpuCommandSimulator commandSimulator,
            IRandomProvider randomProvider,
            SearchProfiler searchProfiler
        )
            : base(
                legalCommandEnumerator,
                commandSimulator,
                randomProvider
            )
        {
            _searchProfiler = Guard.ThrowIfNull(searchProfiler, nameof(searchProfiler));
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

            var rootSearchMoves = ConvertCommandsToSearchMoves(context.State, rootCandidates);
            if (rootSearchMoves.Count == 0)
            {
                onDecided?.Invoke(PickRandomCommand(rootCandidates));
                yield break;
            }

            var totalStopwatch = Stopwatch.StartNew();
            var frameStopwatch = Stopwatch.StartNew();
            TimeSpan timeLimit = TimeSpan.FromMilliseconds(context.SearchTimeLimit.Value);
            TimeSpan frameBudget = TimeSpan.FromMilliseconds(FrameSearchBudgetMilliseconds);
            SearchMove bestMove = rootSearchMoves[0];
            int completedDepth = 0;
            int bestScore = 0;
            bool timeout = false;

            var searchState = new SearchState(context.State);
            _evaluationCache.Clear();
            _searchProfiler.Begin();

            for (int depth = 1; depth <= MaxSearchDepthSafetyLimit; depth++)
            {
                if (IsTimeExpired(totalStopwatch, timeLimit))
                {
                    timeout = true;
                    break;
                }

                SearchIterationResult result = SearchIterationResult.Timeout();
                yield return SearchRoot(
                    context,
                    searchState,
                    rootSearchMoves,
                    depth,
                    totalStopwatch,
                    timeLimit,
                    frameStopwatch,
                    frameBudget,
                    value => result = value
                );

                if (!result.Completed)
                {
                    timeout = true;
                    break;
                }

                bestMove = result.Move;
                bestScore = result.Score;
                completedDepth = depth;

                if (IsWinningScore(result.Score))
                    break;
            }

            SearchProfilerSnapshot snapshot = _searchProfiler.End();
            Debug.Log($"[CPU Search] depth={completedDepth}, {snapshot}, bestScore={bestScore}, timeout={timeout}");

            onDecided?.Invoke(bestMove.ToUseSkillCommand(context.Issuer));
        }

        private IEnumerator SearchRoot(
            CpuAgentDecisionContext context,
            SearchState state,
            IReadOnlyList<SearchMove> candidates,
            int depth,
            Stopwatch totalStopwatch,
            TimeSpan timeLimit,
            Stopwatch frameStopwatch,
            TimeSpan frameBudget,
            Action<SearchIterationResult> onCompleted
        )
        {
            SearchMove bestMove = default;
            bool hasBestMove = false;
            int bestScore = MinScoreSentinel;
            int alpha = MinScoreSentinel;
            const int beta = MaxScoreSentinel;

            foreach (SearchMove candidate in candidates)
            {
                _searchProfiler.RecordNode(0);
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

                SearchUndo undo = state.Apply(candidate);
                int score = AlphaBeta(
                    context,
                    state,
                    context.PlayerId,
                    depth - 1,
                    alpha,
                    beta,
                    totalStopwatch,
                    timeLimit,
                    frameStopwatch,
                    frameBudget,
                    1
                );
                state.Undo(candidate, undo);

                if (IsTimeExpired(totalStopwatch, timeLimit))
                {
                    onCompleted?.Invoke(SearchIterationResult.Timeout());
                    yield break;
                }

                if (!hasBestMove || score > bestScore)
                {
                    bestMove = candidate;
                    bestScore = score;
                    hasBestMove = true;
                }
                alpha = Math.Max(alpha, bestScore);
            }

            onCompleted?.Invoke(hasBestMove
                ? SearchIterationResult.Success(bestMove, bestScore)
                : SearchIterationResult.Timeout());
        }

        private int AlphaBeta(
            CpuAgentDecisionContext context,
            SearchState state,
            PlayerId perspectivePlayerId,
            int depthRemaining,
            int alpha,
            int beta,
            Stopwatch totalStopwatch,
            TimeSpan timeLimit,
            Stopwatch frameStopwatch,
            TimeSpan frameBudget,
            int currentDepth
        )
        {
            _searchProfiler.RecordNode(currentDepth);

            if (ShouldEvaluateCurrentState(depthRemaining, totalStopwatch, timeLimit, frameStopwatch, frameBudget))
                return EvaluateProfiled(context, state, perspectivePlayerId, currentDepth);

            var candidates = EnumerateSearchMoves(context, state);
            if (candidates.Count == 0)
                return EvaluateProfiled(context, state, perspectivePlayerId, currentDepth);

            return IsMaximizingTurn(state, perspectivePlayerId)
                ? SearchMaximizingNode(context, state, perspectivePlayerId, candidates, depthRemaining, alpha, beta, totalStopwatch, timeLimit, frameStopwatch, frameBudget, currentDepth)
                : SearchMinimizingNode(context, state, perspectivePlayerId, candidates, depthRemaining, alpha, beta, totalStopwatch, timeLimit, frameStopwatch, frameBudget, currentDepth);
        }

        private int SearchMaximizingNode(CpuAgentDecisionContext context, SearchState state, PlayerId perspectivePlayerId, IReadOnlyList<SearchMove> candidates, int depthRemaining, int alpha, int beta, Stopwatch totalStopwatch, TimeSpan timeLimit, Stopwatch frameStopwatch, TimeSpan frameBudget, int currentDepth)
        {
            int value = MinScoreSentinel;
            foreach (SearchMove candidate in candidates)
            {
                if (IsTimeExpired(totalStopwatch, timeLimit)) return value;
                if (IsFrameBudgetExpired(frameStopwatch, frameBudget)) return EvaluateProfiled(context, state, perspectivePlayerId, currentDepth);
                SearchUndo undo = state.Apply(candidate);
                int score = AlphaBeta(context, state, perspectivePlayerId, depthRemaining - 1, alpha, beta, totalStopwatch, timeLimit, frameStopwatch, frameBudget, currentDepth + 1);
                state.Undo(candidate, undo);
                value = Math.Max(value, score);
                alpha = Math.Max(alpha, value);
                if (alpha >= beta) break;
            }
            return value == MinScoreSentinel ? EvaluateProfiled(context, state, perspectivePlayerId, currentDepth) : value;
        }

        private int SearchMinimizingNode(CpuAgentDecisionContext context, SearchState state, PlayerId perspectivePlayerId, IReadOnlyList<SearchMove> candidates, int depthRemaining, int alpha, int beta, Stopwatch totalStopwatch, TimeSpan timeLimit, Stopwatch frameStopwatch, TimeSpan frameBudget, int currentDepth)
        {
            int value = MaxScoreSentinel;
            foreach (SearchMove candidate in candidates)
            {
                if (IsTimeExpired(totalStopwatch, timeLimit)) return value;
                if (IsFrameBudgetExpired(frameStopwatch, frameBudget)) return EvaluateProfiled(context, state, perspectivePlayerId, currentDepth);
                SearchUndo undo = state.Apply(candidate);
                int score = AlphaBeta(context, state, perspectivePlayerId, depthRemaining - 1, alpha, beta, totalStopwatch, timeLimit, frameStopwatch, frameBudget, currentDepth + 1);
                state.Undo(candidate, undo);
                value = Math.Min(value, score);
                beta = Math.Min(beta, value);
                if (alpha >= beta) break;
            }
            return value == MaxScoreSentinel ? EvaluateProfiled(context, state, perspectivePlayerId, currentDepth) : value;
        }

        private int EvaluateProfiled(CpuAgentDecisionContext context, SearchState state, PlayerId perspectivePlayerId, int currentDepth)
        {
            _searchProfiler.RecordNode(currentDepth);
            var cacheKey = new SearchEvaluationCacheKey(
                state.CalculateEvaluationHash(),
                perspectivePlayerId.Value,
                context.Evaluator.GetType()
            );

            if (_evaluationCache.TryGetValue(cacheKey, out int cachedScore))
            {
                return cachedScore;
            }

            int score = Evaluate(context, state.ToMatchState(), perspectivePlayerId);
            _evaluationCache[cacheKey] = score;
            return score;
        }

        private List<SearchMove> EnumerateSearchMoves(CpuAgentDecisionContext context, SearchState state)
        {
            MatchState materializedState = state.ToMatchState();
            var materializedContext = CreateDecisionContext(materializedState, context.SearchTimeLimit, context.Evaluator);
            return ConvertCommandsToSearchMoves(materializedState, EnumerateAllLegalCommands(materializedContext));
        }

        private static List<SearchMove> ConvertCommandsToSearchMoves(MatchState state, IReadOnlyList<IMatchCommand> commands)
        {
            var moves = new List<SearchMove>(commands.Count);
            foreach (IMatchCommand command in commands)
            {
                if (TryConvertCommandToSearchMove(state, command, out SearchMove move))
                    moves.Add(move);
            }
            return moves;
        }

        private static bool TryConvertCommandToSearchMove(MatchState state, IMatchCommand command, out SearchMove move)
        {
            if (command is not UseSkillCommand useSkill || !useSkill.Target.HasValue)
            {
                move = default;
                return false;
            }
            if (useSkill.SkillSlotId == BuiltInSkillSlotIds.MovePawn)
            {
                move = SearchMove.MovePawn(useSkill.PlayerId, useSkill.Target.Value);
                return true;
            }
            if (useSkill.SkillSlotId == BuiltInSkillSlotIds.PlaceWall && TryCreateWallPattern(state.Board, useSkill.Target.Value, 3, out var pattern))
            {
                move = SearchMove.PlaceWall(useSkill.PlayerId, pattern.Origin, pattern.Direction, pattern.Cells);
                return true;
            }
            move = default;
            return false;
        }

        private static bool TryCreateWallPattern(BoardState board, Position origin, int length, out WallPlacementPattern pattern)
        {
            if (length <= 0 || !BoardGeometry.IsInside(board, origin) || !BoardGeometry.IsWallLinePosition(origin))
            {
                pattern = default;
                return false;
            }
            var direction = origin.Y % 2 == 1 ? WallDirection.Horizontal : WallDirection.Vertical;
            var cells = new Position[length];
            for (var i = 0; i < length; i++)
            {
                var cell = new Position(origin.X + (direction == WallDirection.Horizontal ? i : 0), origin.Y + (direction == WallDirection.Vertical ? i : 0));
                if (!BoardGeometry.IsInside(board, cell))
                {
                    pattern = default;
                    return false;
                }
                cells[i] = cell;
            }
            pattern = new WallPlacementPattern(origin, direction, length, cells);
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
            int depthRemaining,
            Stopwatch totalStopwatch,
            TimeSpan timeLimit,
            Stopwatch frameStopwatch,
            TimeSpan frameBudget
        )
        {
            return depthRemaining <= 0
                || IsTimeExpired(totalStopwatch, timeLimit)
                || IsFrameBudgetExpired(frameStopwatch, frameBudget);
        }

        private static bool IsMaximizingTurn(SearchState state, PlayerId perspectivePlayerId)
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


        private readonly struct SearchEvaluationCacheKey : IEquatable<SearchEvaluationCacheKey>
        {
            private readonly ulong _stateHash;
            private readonly int _perspectivePlayerValue;
            private readonly Type _evaluatorType;

            public SearchEvaluationCacheKey(ulong stateHash, int perspectivePlayerValue, Type evaluatorType)
            {
                _stateHash = stateHash;
                _perspectivePlayerValue = perspectivePlayerValue;
                _evaluatorType = evaluatorType;
            }

            public bool Equals(SearchEvaluationCacheKey other)
            {
                return _stateHash == other._stateHash
                    && _perspectivePlayerValue == other._perspectivePlayerValue
                    && _evaluatorType == other._evaluatorType;
            }

            public override bool Equals(object obj)
            {
                return obj is SearchEvaluationCacheKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(_stateHash, _perspectivePlayerValue, _evaluatorType);
            }
        }

        private readonly struct SearchIterationResult
        {
            public bool Completed { get; }
            public SearchMove Move { get; }
            public int Score { get; }

            private SearchIterationResult(bool completed, SearchMove move, int score)
            {
                Completed = completed;
                Move = move;
                Score = score;
            }

            public static SearchIterationResult Success(SearchMove move, int score)
            {
                return new SearchIterationResult(true, move, score);
            }

            public static SearchIterationResult Timeout()
            {
                return new SearchIterationResult(false, default, 0);
            }
        }

    }
}
