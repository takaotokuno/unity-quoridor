namespace Quoridor
{
    public interface IGameLogger
    {
        void Log(string message);
        void Warning(string message);
        void Error(string message);
    }
}