using System.Collections.Generic;
using System.Security;
using Newtonsoft.Json;
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
            GameSessionInitViewModel gameSessionState =
                JsonConvert.DeserializeObject<GameSessionInitViewModel>(hubMessageModel.AppendedObject.ToString() ?? string.Empty);
            
            if (gameSessionState != null)
            {
                _gameSessionState.InitializeNewState(gameSessionState);
            }
        }
    }
}