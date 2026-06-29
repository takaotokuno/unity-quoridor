namespace Quoridor
{
    public readonly struct CpuBestCommandSelection
    {
        public bool HasCommand { get; }
        public IMatchCommand Command { get; }
        public int Score { get; }

        private CpuBestCommandSelection(bool hasCommand, IMatchCommand command, int score)
        {
            HasCommand = hasCommand;
            Command = command;
            Score = score;
        }

        public static CpuBestCommandSelection Success(IMatchCommand command, int score)
        {
            return new CpuBestCommandSelection(true, command, score);
        }

        public static CpuBestCommandSelection Empty()
        {
            return new CpuBestCommandSelection(false, null, 0);
        }
    }
}
