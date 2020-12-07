using System.Collections.Generic;
using System.Security;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class SuccessfullyJoinedRoomHandler : IServerMessageHandler 
    {
        private readonly GameSessionState _gameSessionState;

        public SuccessfullyJoinedRoomHandler(GameSessionState gameSessionSateManager)
        {
            _gameSessionState = gameSessionSateManager;
        }
        
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            if (hubMessageModel.AppendedObject != null)
            {
                var roomPlayers = hubMessageModel.AppendedObject as List<GameSessionPlayer>;
                
                // Todo: initialize game session
            }
        }
    }
}