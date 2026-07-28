using System;
using System.Collections.Generic;
using System.Linq;

namespace Quoridor
{
    public sealed class GameDirector
    {
        private readonly MatchFactory _matchFactory;
        private readonly List<MatchLifetimeScope> _matchScopes;
        public IReadOnlyList<MatchSession> MatchList =>
            _matchScopes.Select(scope => scope.Session).ToArray();
        public bool HasMatch => _matchScopes.Count > 0;
        public MatchSession FirstMatch => HasMatch ? _matchScopes[0].Session : null;

        public GameDirector(MatchFactory matchFactory)
        {
            _matchFactory = matchFactory;
            _matchScopes = new();
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
            MatchLifetimeScope scope = _matchFactory.Create(setting);
            MatchSession match = scope.Session;
            _matchScopes.Add(scope);
            return match;
        }

        private IGameResponse _HandleResetRequest(ResetRequest request)
        {
            _Reset();
            return new ResetResponse();
        }

        private void _Reset()
        {
            foreach(var scope in _matchScopes)
            {
                scope.Dispose();
            }
            _matchScopes.Clear();
        }
    }
}
