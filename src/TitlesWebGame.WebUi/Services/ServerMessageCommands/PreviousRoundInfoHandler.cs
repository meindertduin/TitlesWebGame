using System;
using Newtonsoft.Json;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class PreviousRoundInfoHandler : IServerMessageHandler 
    {
        private readonly GameSessionState _gameSessionState;

        public PreviousRoundInfoHandler(GameSessionState gameSessionState)
        {
            _gameSessionState = gameSessionState;
        }
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            var sessionStateUpdateInfo =
                JsonConvert.DeserializeObject<SessionStateUpdateViewModel>(hubMessageModel.AppendedObject.ToString() ??
                                                                           String.Empty);
            if (sessionStateUpdateInfo != null)
            {
                _gameSessionState.SetSessionGameStatUpdateInfo(sessionStateUpdateInfo);
            }
        }
    }
}