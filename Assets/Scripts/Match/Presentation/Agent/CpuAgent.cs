using System;
using System.Collections;
using UnityEngine;

namespace Quoridor
{
    /// <summary>
    /// CPU プレイヤーのターン進行を担当する MonoBehaviour。
    /// Strategy からコマンドを受け取り、ターンが進むまで再試行またはスキップを発行する。
    /// </summary>
    public sealed class CpuAgent
        : MonoBehaviour,
          IMatchObserver<TurnStartedEvent>,
          IDisposable
    {
        private const int MaxCommandAttemptsPerTurn = 3;
        private const float TurnProgressCheckDelaySeconds = 0.2f;

        private PlayerId _playerId;
        private IMatchCommandPort _commandPort;
        private IMatchEventBus _eventBus;
        private MatchState _state;
        private CpuAgentBehavior _behavior;
        private int _activeTurn = -1;
        private int _attemptCount;
        private int _decisionVersion;
        private bool _turnSkipDispatched;
        private bool _isDisposed;
        private Coroutine _turnProgressCheckCoroutine;
        private Coroutine _decisionCoroutine;

        /// <summary>
        /// MatchFactory から呼ばれる初期化メソッド。
        /// </summary>
        public void Initialize(
            PlayerId playerId,
            MatchState state,
            IMatchCommandPort commandPort,
            IMatchEventBus eventBus,
            CpuAgentBehavior behavior
        )
        {
            _playerId = playerId;
            _state = Guard.ThrowIfNull(state, nameof(state));
            _commandPort = Guard.ThrowIfNull(commandPort, nameof(commandPort));
            _eventBus = Guard.ThrowIfNull(eventBus, nameof(eventBus));
            _behavior = Guard.ThrowIfNull(behavior, nameof(behavior));

            _eventBus.Subscribe<TurnStartedEvent>(this);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _eventBus?.Unsubscribe<TurnStartedEvent>(this);
            StopTurnProgressCheck();
            StopCommandDecision();
            _isDisposed = true;
        }

        /// <summary>
        /// ターン開始時に呼ばれる。担当プレイヤーのターンなら Strategy で行動を決定・発行する。
        /// </summary>
        public void OnTurnStarted(MatchState state, PlayerId playerId)
        {
            if (playerId != _playerId) return;
            if (state == null) return;

            _activeTurn = state.CurrentTurn;
            _attemptCount = 0;
            _decisionVersion++;
            _turnSkipDispatched = false;
            StopTurnProgressCheck();
            StartCommandDecision(state, _decisionVersion);
        }

        private void StartCommandDecision(MatchState state, int decisionVersion)
        {
            StopCommandDecision();
            _decisionCoroutine = StartCoroutine(DecideAndDispatchCommand(state, decisionVersion));
        }

        private void StopCommandDecision()
        {
            if (_decisionCoroutine == null) return;

            StopCoroutine(_decisionCoroutine);
            _decisionCoroutine = null;
        }

        private IEnumerator DecideAndDispatchCommand(MatchState state, int decisionVersion)
        {
            // TurnStartedEvent の配布中に探索を始めず、次の Observer へ即座に制御を返す。
            yield return null;

            if (!CanDecideCommand(state, decisionVersion))
            {
                _decisionCoroutine = null;
                yield break;
            }

            IMatchCommand command = null;
            yield return StartCoroutine(_behavior.DecideCommand(
                state,
                _playerId,
                MatchCommandIssuers.CpuAgent,
                decidedCommand => command = decidedCommand
            ));

            _decisionCoroutine = null;

            if (!CanDecideCommand(state, decisionVersion))
                yield break;

            DispatchDecidedCommand(command);
        }

        private bool CanDecideCommand(MatchState state, int decisionVersion)
        {
            return state != null
                && decisionVersion == _decisionVersion
                && IsActiveCpuTurn()
                && _attemptCount < MaxCommandAttemptsPerTurn;
        }

        private void DispatchDecidedCommand(IMatchCommand command)
        {
            if (command == null)
            {
                DispatchTurnSkipIfTurnIsActive();
                return;
            }

            _attemptCount++;
            _commandPort.DispatchCommand(command);
            ScheduleTurnProgressCheck();
        }

        private void DispatchTurnSkipIfTurnIsActive()
        {
            if (_turnSkipDispatched) return;
            if (!IsActiveCpuTurn()) return;

            _turnSkipDispatched = true;
            StopTurnProgressCheck();
            _commandPort.DispatchCommand(new TurnSkipCommand(
                _playerId,
                MatchCommandIssuers.CpuAgent
            ));
        }

        /// <summary>
        /// CommandPort は非同期キューなので、Dispatch 直後ではなく少し待ってから
        /// 同じ CPU ターンが継続しているかを確認する。
        /// </summary>
        private void ScheduleTurnProgressCheck()
        {
            StopTurnProgressCheck();
            _turnProgressCheckCoroutine = StartCoroutine(CheckTurnProgressAfterDelay(_decisionVersion));
        }

        private void StopTurnProgressCheck()
        {
            if (_turnProgressCheckCoroutine == null) return;

            StopCoroutine(_turnProgressCheckCoroutine);
            _turnProgressCheckCoroutine = null;
        }

        private IEnumerator CheckTurnProgressAfterDelay(int decisionVersion)
        {
            yield return new WaitForSecondsRealtime(TurnProgressCheckDelaySeconds);

            _turnProgressCheckCoroutine = null;

            if (decisionVersion != _decisionVersion) yield break;
            if (!IsActiveCpuTurn()) yield break;

            if (_attemptCount >= MaxCommandAttemptsPerTurn)
            {
                DispatchTurnSkipIfTurnIsActive();
                yield break;
            }

            StartCommandDecision(_state, _decisionVersion);
        }

        private bool IsActiveCpuTurn()
        {
            return _state != null
                && _state.IsInProgress
                && _state.CurrentPlayerId == _playerId
                && _state.CurrentTurn == _activeTurn;
        }

        /// <summary>
        /// MatchEventBus から TurnStartedEvent を受け取り、自分のターンなら行動する。
        /// </summary>
        public void Notify(TurnStartedEvent e)
        {
            OnTurnStarted(_state, e.PlayerId);
        }
    }
}
