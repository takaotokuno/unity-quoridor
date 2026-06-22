using System.Collections;
using UnityEngine;

namespace Quoridor
{
    /// <summary>
    /// ml-agents の Agent クラスを継承する想定の CPU エージェント（スタブ実装）。
    /// 現在は ml-agents パッケージなしでコンパイルできるよう、
    /// MonoBehaviour のみを継承し、ml-agents 固有の型は #if MLAGENTS ガードで保護している。
    ///
    /// 実際に ml-agents を導入する場合は以下の手順を踏む:
    ///   1. Unity Package Manager で com.unity.ml-agents を追加する
    ///   2. #define MLAGENTS をプロジェクト設定に追加する
    ///   3. class 宣言を `MonoBehaviour` から `Unity.MLAgents.Agent` に変更する
    ///   4. CollectObservations / OnActionReceived の override キーワードを有効にする
    /// </summary>
    public sealed class MlAgentsCpuAgent
        : MonoBehaviour,
          ICpuAgent,
          IMatchObserver<TurnStartedEvent>
    {
        // ---------------------------------------------------------------
        // 依存関係（Initialize() で注入）
        // ---------------------------------------------------------------

        private const int MaxCommandAttemptsPerTurn = 3;
        private const float TurnProgressCheckDelaySeconds = 0.2f;

        private PlayerId _playerId;
        private IMatchCommandPort _commandPort;
        private IMatchEventBus _eventBus;
        private MatchState _state;
        private ICpuAgentStrategy _strategy;
        private int _activeTurn = -1;
        private int _attemptCount;
        private int _decisionVersion;
        private bool _turnSkipDispatched;
        private Coroutine _turnProgressCheckCoroutine;

        // ---------------------------------------------------------------
        // 初期化
        // ---------------------------------------------------------------

        /// <summary>
        /// MatchFactory から呼ばれる初期化メソッド。
        /// </summary>
        public void Initialize(
            PlayerId playerId,
            MatchState state,
            IMatchCommandPort commandPort,
            IMatchEventBus eventBus,
            ICpuAgentStrategy strategy
        )
        {
            _playerId = playerId;
            _state = Guard.ThrowIfNull(state, nameof(state));
            _commandPort = Guard.ThrowIfNull(commandPort, nameof(commandPort));
            _eventBus = Guard.ThrowIfNull(eventBus, nameof(eventBus));
            _strategy = Guard.ThrowIfNull(strategy, nameof(strategy));

            _eventBus.Subscribe<TurnStartedEvent>(this);
        }

        private void OnDestroy()
        {
            _eventBus?.Unsubscribe<TurnStartedEvent>(this);
            StopTurnProgressCheck();
        }

        // ---------------------------------------------------------------
        // ICpuAgent
        // ---------------------------------------------------------------

        /// <inheritdoc/>
        public void OnTurnStarted(MatchState state, PlayerId playerId)
        {
            if (playerId != _playerId) return;
            if (state == null) return;

            _activeTurn = state.CurrentTurn;
            _attemptCount = 0;
            _decisionVersion++;
            _turnSkipDispatched = false;
            StopTurnProgressCheck();

            TryDispatchCommandIfTurnIsActive(state);
        }

        private void TryDispatchCommandIfTurnIsActive(MatchState state)
        {
            if (state == null) return;
            if (!IsActiveCpuTurn()) return;
            if (_attemptCount >= MaxCommandAttemptsPerTurn) return;

            var context = new CpuAgentDecisionContext(state, _playerId, MatchCommandIssuers.CpuAgent);
            var command = _strategy.DecideCommand(context);
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

            TryDispatchCommandIfTurnIsActive(_state);
        }

        private bool IsActiveCpuTurn()
        {
            return _state != null
                && _state.IsInProgress
                && _state.CurrentPlayerId == _playerId
                && _state.CurrentTurn == _activeTurn;
        }


        // ---------------------------------------------------------------
        // IMatchObserver<TurnStartedEvent>
        // ---------------------------------------------------------------

        /// <summary>
        /// MatchEventBus から TurnStartedEvent を受け取り、自分のターンなら行動する。
        /// </summary>
        public void Notify(TurnStartedEvent e)
        {
            OnTurnStarted(_state, e.PlayerId);
        }


        // ---------------------------------------------------------------
        // ml-agents スタブ（#if MLAGENTS で保護）
        // ---------------------------------------------------------------

#if MLAGENTS
        // TODO: ml-agents を導入した際は以下の override を有効にする。
        //       クラス宣言も MonoBehaviour → Unity.MLAgents.Agent に変更すること。

        /// <summary>
        /// ml-agents の観測収集メソッド（スタブ）。
        /// VectorSensor に以下の観測値を追加する予定:
        ///   - ボードグリッド状態（壁の有無）
        ///   - 各プレイヤーのコマ位置（X, Y）
        ///   - 各スキルのクールダウン残数
        /// </summary>
        public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor)
        {
            // TODO: ボードグリッド状態を追加する
            // var grid = _state.Board.Grid;
            // int size = grid.GetLength(0);
            // for (int y = 0; y < size; y++)
            //     for (int x = 0; x < size; x++)
            //         sensor.AddObservation(grid[y, x]);

            // TODO: コマ位置を追加する
            // foreach (var pawn in _state.Board.Pawns)
            // {
            //     sensor.AddObservation(pawn.X);
            //     sensor.AddObservation(pawn.Y);
            // }

            // TODO: スキルクールダウンを追加する
            // var player = _state.GetPlayer(_playerId);
            // foreach (var skill in player.Skills.Values)
            //     sensor.AddObservation(skill.CoolDownRemaining);
        }

        /// <summary>
        /// ml-agents の行動受信メソッド（スタブ）。
        /// ActionBuffers から行動インデックスを読み取り、対応するコマンドを
        /// IMatchCommandPort に発行する予定:
        ///   - actions.DiscreteActions[0]: 行動種別（0=コマ移動, 1=壁配置, 2=スキル使用）
        ///   - actions.DiscreteActions[1]: ターゲット位置 X
        ///   - actions.DiscreteActions[2]: ターゲット位置 Y
        ///   - actions.DiscreteActions[3]: スキルスロットID（スキル使用時）
        /// </summary>
        public override void OnActionReceived(Unity.MLAgents.Actuators.ActionBuffers actions)
        {
            // TODO: 行動バッファからコマンドを生成して発行する
            // int actionType = actions.DiscreteActions[0];
            // int targetX    = actions.DiscreteActions[1];
            // int targetY    = actions.DiscreteActions[2];
            // int skillSlot  = actions.DiscreteActions[3];
            // var target = new Position(targetX, targetY);
            //
            // IMatchCommand command = actionType switch
            // {
            //     0 => new MovePawnCommand(_playerId, target, MatchCommandIssuers.CpuAgent),
            //     1 => new UseSkillCommand(_playerId, SkillId.NormalBuildWall,
            //              BuiltInSkillSlotIds.BuildWall, target, MatchCommandIssuers.CpuAgent),
            //     2 => BuildSkillCommand(skillSlot, target),
            //     _ => null
            // };
            //
            // if (command != null)
            //     _commandPort.DispatchCommand(command);
        }
#else
        // ml-agents パッケージが未導入の場合のスタブ（コンパイルエラー回避用）

        /// <summary>
        /// ml-agents の CollectObservations に相当するスタブ。
        /// ml-agents 導入後は override に変更し、VectorSensor に以下を追加する:
        ///   - ボードグリッド状態（壁の有無）
        ///   - 各プレイヤーのコマ位置（X, Y）
        ///   - 各スキルのクールダウン残数
        /// </summary>
        /// <param name="sensor">
        /// ml-agents の VectorSensor（現在は object 型でスタブ化）
        /// </param>
        public void CollectObservations(object sensor)
        {
            // TODO: ml-agents 導入後に実装する
            // sensor.AddObservation(grid[y, x]);          // ボードグリッド状態
            // sensor.AddObservation(pawn.X / boardSize);  // コマ位置 X（正規化）
            // sensor.AddObservation(pawn.Y / boardSize);  // コマ位置 Y（正規化）
            // sensor.AddObservation(skill.CoolDownRemaining); // スキルクールダウン
        }

        /// <summary>
        /// ml-agents の OnActionReceived に相当するスタブ。
        /// ml-agents 導入後は override に変更し、ActionBuffers から以下を読み取る:
        ///   - DiscreteActions[0]: 行動種別（0=コマ移動, 1=壁配置, 2=スキル使用）
        ///   - DiscreteActions[1]: ターゲット位置 X
        ///   - DiscreteActions[2]: ターゲット位置 Y
        ///   - DiscreteActions[3]: スキルスロットID（スキル使用時）
        /// </summary>
        /// <param name="actions">
        /// ml-agents の ActionBuffers（現在は object 型でスタブ化）
        /// </param>
        public void OnActionReceived(object actions)
        {
            // TODO: ml-agents 導入後に実装する
            // int actionType = actions.DiscreteActions[0];
            // int targetX    = actions.DiscreteActions[1];
            // int targetY    = actions.DiscreteActions[2];
            // int skillSlot  = actions.DiscreteActions[3];
            // var target = new Position(targetX, targetY);
            //
            // IMatchCommand command = actionType switch
            // {
            //     0 => new MovePawnCommand(_playerId, target, MatchCommandIssuers.CpuAgent),
            //     1 => new UseSkillCommand(_playerId, SkillId.NormalBuildWall,
            //              BuiltInSkillSlotIds.BuildWall, target, MatchCommandIssuers.CpuAgent),
            //     2 => BuildSkillCommand(skillSlot, target),
            //     _ => null
            // };
            //
            // if (command != null)
            //     _commandPort.DispatchCommand(command);
        }
#endif
    }
}
