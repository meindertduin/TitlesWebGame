using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class RoundReviewMessageHandler : IServerMessageHandler
    {
        private readonly GameSessionState _gameSessionState;

        public RoundReviewMessageHandler(GameSessionState gameSessionState)
        {
            _gameSessionState = gameSessionState;
        }
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            var roundReviewInfo =
                JsonConvert.DeserializeObject<RoundReviewMessageModel>(hubMessageModel.AppendedObject.ToString() ??
                                                                       String.Empty);

            if (roundReviewInfo != null)
            {
                _gameSessionState.SetRoundReviewStates(roundReviewInfo);
            }
        }
    }
}