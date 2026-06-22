namespace Quoridor
{
    public sealed class BoardGamePort
    {
        private GameDirector _gameDirector;

        public BoardGamePort(GameDirector gameDirector)
        {
            _gameDirector = gameDirector;    
        }

        public IMatchResponse SendMatchCommand(IMatchCommand command)
        {
            var match = _gameDirector.FirstMatch;
            if(match == null)
            {
                return new CommandRejectedResponse("Director doesn't have match");
            }

            var result = match.DispatchCommand(command);
            return result;
        }

        public IGameResponse SendGameRequest(IGameRequest request)
        {
            return _gameDirector.DispatchRequest(request);
        }
    }
}