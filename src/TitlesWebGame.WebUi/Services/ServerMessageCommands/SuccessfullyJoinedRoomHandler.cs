using System.Collections.Generic;
using System.Security;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class SuccessfullyJoinedRoomHandler : IServerMessageHandler 
    {
        private readonly GameSessionSateManager _gameSessionSateManager;

        public SuccessfullyJoinedRoomHandler(GameSessionSateManager gameSessionSateManager)
        {
            _gameSessionSateManager = gameSessionSateManager;
        }
        
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            if (hubMessageModel.AppendedObject != null)
            {
                var roomPlayers = hubMessageModel.AppendedObject as List<GameSessionPlayer>;
                _gameSessionSateManager.SetPlayersState(roomPlayers);
                
            }
        }
    }
}