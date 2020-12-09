using System;
using Newtonsoft.Json;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class TitlesRoundEndedHandler : IServerMessageHandler
    {
        private readonly GameSessionState _gameSessionState;

        public TitlesRoundEndedHandler(GameSessionState gameSessionState)
        {
            _gameSessionState = gameSessionState;
        }
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            var titlesRoundResult =
                JsonConvert.DeserializeObject<TitlesRoundResults>(hubMessageModel.AppendedObject.ToString() ??
                                                                            String.Empty);

            if (titlesRoundResult != null)
            {
                _gameSessionState.EndTitleRound(titlesRoundResult);
            }
        }
    }
}