using System;
using Newtonsoft.Json;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class SessionEndedHandler : IServerMessageHandler
    {
        private readonly GameSessionState _gameSessionState;

        public SessionEndedHandler(GameSessionState gameSessionState)
        {
            _gameSessionState = gameSessionState;
        }
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            var endResults =
                JsonConvert.DeserializeObject<TitlesGameEndSessionResults>(hubMessageModel.AppendedObject.ToString() ??
                                                                           String.Empty);

            if (endResults != null)
            {
                _gameSessionState.EndSession(endResults);
            }
        }
    }
}