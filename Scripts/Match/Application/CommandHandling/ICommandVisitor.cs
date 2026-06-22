namespace Quoridor
{
    public interface ICommandVisitor
    {
        CommandResult Visit(PlaceWallCommand command);
        CommandResult Visit(MovePawnCommand command);
        CommandResult Visit(UseSkillCommand command);
        CommandResult Visit(MatchStartCommand command);
        CommandResult Visit(ResignCommand command);
        CommandResult Visit(SkipCommand command);
        CommandResult Visit(TurnSkipCommand command);
    }
}
