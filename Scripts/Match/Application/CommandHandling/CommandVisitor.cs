namespace Quoridor
{
    public sealed class CommandVisitor : ICommandVisitor
    {
        private readonly MatchState _state;
        private readonly UseSkillCommandHandler _useSkillCommandHandler;
        private readonly MatchControlCommandHandler _matchControlCommandHandler;

        public CommandVisitor(
            MatchState state,
            UseSkillCommandHandler useSkillCommandHandler,
            MatchControlCommandHandler matchControlCommandHandler
        )
        {
            _state = Guard.ThrowIfNull(state, nameof(state));
            _useSkillCommandHandler = Guard.ThrowIfNull(
                useSkillCommandHandler,
                nameof(useSkillCommandHandler)
            );
            _matchControlCommandHandler = Guard.ThrowIfNull(
                matchControlCommandHandler,
                nameof(matchControlCommandHandler)
            );
        }

        public CommandResult Visit(PlaceWallCommand command)
        {
            Guard.ThrowIfNull(command, nameof(command));

            var result = _state.Board.PlaceWall(command.Targets);

            return new CommandResult(
                result.Events,
                consumeTurn: false
            );
        }

        public CommandResult Visit(MovePawnCommand command)
        {
            Guard.ThrowIfNull(command, nameof(command));

            var result = _state.Board.MovePawn(command.PlayerId, command.To);

            return new CommandResult(
                result.Events,
                consumeTurn: false
            );
        }

        public CommandResult Visit(UseSkillCommand command)
        {
            return _useSkillCommandHandler.Handle(command);
        }

        public CommandResult Visit(ResignCommand command)
        {
            return _matchControlCommandHandler.Handle(command);
        }

        public CommandResult Visit(MatchStartCommand command)
        {
            return _matchControlCommandHandler.Handle(command);
        }

        public CommandResult Visit(SkipCommand command)
        {
            return _matchControlCommandHandler.Handle(command);
        }

        public CommandResult Visit(TurnSkipCommand command)
        {
            return _matchControlCommandHandler.Handle(command);
        }
    }
}
