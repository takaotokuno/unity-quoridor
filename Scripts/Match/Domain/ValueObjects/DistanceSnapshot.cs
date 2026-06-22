namespace Quoridor
{
    public readonly struct DistanceSnapshot
    {
        public int FirstDistance { get; }
        public int SecondDistance { get; }

        public DistanceSnapshot(
            int firstDistance,
            int secondDistance
        )
        {
            FirstDistance = firstDistance;
            SecondDistance = secondDistance;
        }

        public int GetDistance(PlayerId playerId)
        {
            if (playerId.IsFirstPlayer)
            {
                return FirstDistance;
            }

            if (playerId.IsSecondPlayer)
            {
                return SecondDistance;
            }

            return int.MaxValue;
        }
    }
}
