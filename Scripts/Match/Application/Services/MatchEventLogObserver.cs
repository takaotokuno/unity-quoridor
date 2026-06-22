using System.Text;

namespace Quoridor
{
    public sealed class MatchEventLogObserver
        : IMatchObserver<CheckmateEvent>,
          IMatchObserver<MatchFinishedEvent>,
          IMatchObserver<MatchReadiedEvent>,
          IMatchObserver<MatchStartedEvent>,
          IMatchObserver<StateRestoredEvent>,
          IMatchObserver<TurnEndedEvent>,
          IMatchObserver<TurnSkippedEvent>,
          IMatchObserver<TurnStartedEvent>,
          IMatchObserver<SkillSelectionChangedEvent>,
          IMatchObserver<SkillUsedEvent>,
          IMatchObserver<StatusAddedEvent>,
          IMatchObserver<StatusAppliedEvent>,
          IMatchObserver<StatusRemovedEvent>,
          IMatchObserver<CommandRejectedEvent>,
          IMatchObserver<InputRejectedEvent>,
          IMatchObserver<WallPlacedEvent>,
          IEventSubscriber
    {
        private IMatchEventBus _eventBus;
        private readonly IGameLogger _logger;
        private readonly MatchState _state;

        public MatchEventLogObserver(
            IGameLogger logger,
            MatchState state
        )
        {
            _logger = logger;
            _state = state;
        }

        public void Notify(CheckmateEvent e)
        {
            LogEvent(e);
        }

        public void Notify(MatchFinishedEvent e)
        {
            LogEvent(e);
        }

        public void Notify(MatchReadiedEvent e)
        {
            LogEvent(e);
        }

        public void Notify(MatchStartedEvent e)
        {
            LogEvent(e);
        }

        public void Notify(StateRestoredEvent e)
        {
            LogEvent(e);
        }

        public void Notify(TurnEndedEvent e)
        {
            LogEvent(e);
        }

        public void Notify(TurnSkippedEvent e)
        {
            LogEvent(e);
        }

        public void Notify(TurnStartedEvent e)
        {
            LogEvent(e);
            _logger.Log(BuildMatchStateLog());
        }

        public void Notify(SkillSelectionChangedEvent e)
        {
            LogEvent(e);
        }

        public void Notify(SkillUsedEvent e)
        {
            LogEvent(e);
        }

        public void Notify(StatusAddedEvent e)
        {
            LogEvent(e);
        }

        public void Notify(StatusAppliedEvent e)
        {
            LogEvent(e);
        }

        public void Notify(StatusRemovedEvent e)
        {
            LogEvent(e);
        }

        public void Notify(CommandRejectedEvent e)
        {
            LogWarningEvent(e);
        }
        
        public void Notify(InputRejectedEvent e)
        {
            LogEvent(e);
        }

        public void Notify(WallPlacedEvent e)
        {
            LogEvent(e);   
        }

        public void SubscribeTo(IMatchEventBus eventBus)
        {
            _eventBus = eventBus;

            _eventBus.Subscribe<CheckmateEvent>(this);
            _eventBus.Subscribe<MatchFinishedEvent>(this);
            _eventBus.Subscribe<MatchReadiedEvent>(this);
            _eventBus.Subscribe<MatchStartedEvent>(this);
            _eventBus.Subscribe<StateRestoredEvent>(this);
            _eventBus.Subscribe<TurnEndedEvent>(this);
            _eventBus.Subscribe<TurnSkippedEvent>(this);
            _eventBus.Subscribe<TurnStartedEvent>(this);
            _eventBus.Subscribe<SkillSelectionChangedEvent>(this);
            _eventBus.Subscribe<SkillUsedEvent>(this);
            _eventBus.Subscribe<StatusAddedEvent>(this);
            _eventBus.Subscribe<StatusAppliedEvent>(this);
            _eventBus.Subscribe<StatusRemovedEvent>(this);
            _eventBus.Subscribe<CommandRejectedEvent>(this);
            _eventBus.Subscribe<InputRejectedEvent>(this);
            _eventBus.Subscribe<WallPlacedEvent>(this);
        }

        private void LogEvent<T>(T e)
        {
            _logger.Log(LogFormatter.Format("Event", e));
        }

        private void LogWarningEvent<T>(T e)
        {
            _logger.Warning(LogFormatter.Format("Event", e));
        }

        private string BuildMatchStateLog()
        {
            var sb = new StringBuilder();

            sb.AppendLine("========== TurnStarted Debug ==========");
            sb.AppendLine($"Phase: {_state.Phase}");
            sb.AppendLine($"CurrentPlayerId: {_state.CurrentPlayerId}");

            AppendTurnState(sb);
            AppendBoardState(sb);
            AppendPlayers(sb);

            sb.AppendLine("=======================================");

            return sb.ToString();
        }

        private void AppendTurnState(StringBuilder sb)
        {
            sb.AppendLine("--- TurnState ---");
            sb.AppendLine($"CurrentPlayerId: {_state.Turn.CurrentPlayerId}");
            sb.AppendLine($"TurnNumber: {_state.Turn.CurrentTurn}");
        }

        private void AppendBoardState(StringBuilder sb)
        {
            sb.AppendLine("--- BoardState ---");

            var grid = _state.Board.Grid;

            sb.AppendLine($"Grid Width: {grid.Width}");
            sb.AppendLine($"Grid Height: {grid.Height}");

            sb.AppendLine("Cells:");

            for (int y = 0; y < grid.Height; y++)
            {
                var row = new StringBuilder();
                for (int x = 0; x < grid.Width; x++)
                {
                    var cell = grid.Get(x, y);
                    row.Append(cell);
                }
                sb.AppendLine(row.ToString());
            }
        }

        private void AppendPlayers(StringBuilder sb)
        {
            sb.AppendLine("--- Players ---");

            foreach (PlayerState player in _state.Players)
            {
                AppendPlayer(sb, player);
            }
        }

        private void AppendPlayer(StringBuilder sb, PlayerState player)
        {
            sb.AppendLine($"Player: {player.PlayerId}");
            sb.AppendLine($"  IsActive: {player.IsActive}");

            sb.AppendLine("  Runtime:");
            sb.AppendLine($" CanAct: {player.Runtime.CanAct}");

            sb.AppendLine("  Skills:");

            foreach (var pair in player.Skills)
            {
                SkillSlotId slotId = pair.Key;
                SkillState skill = pair.Value;

                sb.AppendLine(
                    $"    Slot={slotId}, " +
                    $"SkillId={skill.SkillId}, " +
                    $"RemainingUses={skill.RemainingUses}, " +
                    $"CoolDownRemaining={skill.CoolDownRemaining}, " +
                    $"Charge={skill.Charge}"
                );
            }

            sb.AppendLine("  Statuses:");

            foreach (StatusState status in player.Statuses)
            {
                sb.AppendLine(
                    $"    StatusId={status.StatusId}, " +
                    $"RemainingTurns={status.RemainingTurns}"
                );
            }
        }
    }
}
