using System;
using Newtonsoft.Json;
using TitlesWebGame.Domain.Entities;
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
            var previousRoundInfo =
                JsonConvert.DeserializeObject<GameRoundInfo>(hubMessageModel.AppendedObject.ToString() ??
                                                                           String.Empty);
            if (previousRoundInfo != null)
            {
                _gameSessionState.UpdatePreviousRound(previousRoundInfo);
            }
        }
    }
}