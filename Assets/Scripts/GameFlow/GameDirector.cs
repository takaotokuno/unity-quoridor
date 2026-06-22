using System;
using System.Collections.Generic;

namespace Quoridor
{
    public sealed class GameDirector
    {
        private MatchFactory _matchFactory;
        private List<MatchSession> _matchList;
        public IReadOnlyList<MatchSession> MatchList => _matchList;
        public bool HasMatch => MatchList.Count > 0;
        public MatchSession FirstMatch => HasMatch ? _matchList[0] : null;

        public GameDirector(MatchFactory matchFactory)
        {
            _matchFactory = matchFactory;
            _matchList = new();
        }

        public IGameResponse DispatchRequest(IGameRequest request)
        {
            if(request == null)
            {
                return new GameErrorResponse("Request is null");   
            }

            try
            {
                return request switch
                {
                    NewSessionRequest r => _HandleNewSessionRequest(r),
                    ResetRequest r => _HandleResetRequest(r),
                    _ => new GameErrorResponse("Unsupported request type") 
                };
            }
            catch(Exception e)
            {
                UnityEngine.Debug.LogException(e);
                return new GameErrorResponse(e.Message);
            } 
        }

        private IGameResponse _HandleNewSessionRequest(NewSessionRequest request)
        {
            if(request.Setting == null)
            {
                return new GameErrorResponse("MatchSetting is null");
            }
            MatchSession match = _CreateMatch(request.Setting);
            return new NewSessionResponse(match);
        }

        private MatchSession _CreateMatch(MatchSetting setting)
        {
            MatchSession match = _matchFactory.Create(setting);
            _matchList.Add(match);
            return match;
        }

        private IGameResponse _HandleResetRequest(ResetRequest request)
        {
            _Reset();
            return new ResetResponse();
        }

        private void _Reset()
        {
            foreach(var match in _matchList)
            {
                match.Dispose();   
            }
            _matchList.Clear();
        }
    }
}