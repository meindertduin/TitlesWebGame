using System;
using Newtonsoft.Json;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.Enums;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class NextRoundInfoHandler : IServerMessageHandler
    {
        private readonly GameSessionState _gameSessionState;

        public NextRoundInfoHandler(GameSessionState gameSessionState)
        {
            _gameSessionState = gameSessionState;
        }
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            var nextRoundType =
                JsonConvert.DeserializeObject<GameRoundInfoViewModel>(hubMessageModel.AppendedObject.ToString() ??
                                                                      String.Empty).GameRoundsType;

            var deserializer = new RoundInfoDeserializer();
            GameRoundInfoViewModel nextRoundInfo = deserializer
                .DeserializeViewModel(hubMessageModel.AppendedObject.ToString() ?? String.Empty, nextRoundType);
            
            if (nextRoundInfo != null)
            {
                _gameSessionState.SetNextRoundInfo(nextRoundInfo);
                Console.WriteLine("next round info loaded");
            }
        }
    }
}