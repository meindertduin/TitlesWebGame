using System.Collections.Generic;
using TitlesWebGame.Domain.Entities;

namespace TitlesWebGame.WebUi.Services
{
    public class GameSessionSateManager
    {

        private GameSessionState _gameSessionState;

        public void SetGame(string roomKey)
        {
            _gameSessionState = new GameSessionState()
            {
                RoomKey = roomKey,
            };
        }

        public void SetPlayersState(List<GameSessionPlayer> playersStates)
        {
            _gameSessionState.SetPlayerStates(playersStates);
        }

        public void AddPlayer(GameSessionPlayer player)
        {
            _gameSessionState.AddPlayer(player);
        }

        public void RemovePlayer(string connectionId)
        {
            _gameSessionState.RemovePlayer(connectionId);
        }
    }
}