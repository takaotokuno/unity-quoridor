namespace Quoridor
{
    public sealed record DistanceUpdatedEvent : MatchEventBase
    {
        public DistanceSnapshot Distances { get; }

        public DistanceUpdatedEvent(DistanceSnapshot distances)
        {
            Distances = distances;
        }

        public int GetDistance(PlayerId playerId)
        {
            return playerId.IsFirstPlayer
                ? Distances.FirstDistance
                : Distances.SecondDistance;
        }

        public override void Dispatch(IMatchEventBus bus) => bus.DispatchEvent(this);
    }
}
