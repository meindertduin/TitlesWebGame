using System;
using Newtonsoft.Json;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class PlayerLeftGroupHandler : IServerMessageHandler
    {
        private readonly GameSessionState _gameSessionState;

        public PlayerLeftGroupHandler(GameSessionState gameSessionSateManager)
        {
            _gameSessionState = gameSessionSateManager;
        }
        
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            string leavingConnectionId =
                JsonConvert.DeserializeObject<string>(hubMessageModel.AppendedObject.ToString() ?? string.Empty);

            if (String.IsNullOrEmpty(leavingConnectionId) == false)
            {
                _gameSessionState.RemovePlayer(leavingConnectionId);
            }
        }
    }
}