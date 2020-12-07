using System;
using Newtonsoft.Json;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class PlayerJoinedGroupHandler : IServerMessageHandler
    {
        private readonly GameSessionState _gameSessionState;

        public PlayerJoinedGroupHandler(GameSessionState gameSessionSateManager)
        {
            _gameSessionState = gameSessionSateManager;
        }
        
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            var joiningGameSessionPlayer = JsonConvert.DeserializeObject<GameSessionPlayer>(
                hubMessageModel.AppendedObject.ToString() ?? String.Empty);
            
            if (joiningGameSessionPlayer != null)
            {
                _gameSessionState.AddPlayer(joiningGameSessionPlayer);
            }
        }
    }
}