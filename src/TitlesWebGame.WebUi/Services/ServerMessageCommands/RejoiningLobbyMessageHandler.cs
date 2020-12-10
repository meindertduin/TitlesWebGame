using System;
using Newtonsoft.Json;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class RejoiningLobbyMessageHandler : IServerMessageHandler
    {
        private readonly GameSessionState _gameSessionState;

        public RejoiningLobbyMessageHandler(GameSessionState gameSessionState)
        {
            _gameSessionState = gameSessionState;
        }
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            _gameSessionState.PlayAgain();
        }
    }
}