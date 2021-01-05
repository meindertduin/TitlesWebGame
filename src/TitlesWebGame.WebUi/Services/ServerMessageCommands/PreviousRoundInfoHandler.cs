using System;
using Newtonsoft.Json;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.Enums;
using TitlesWebGame.Domain.ViewModels;
using TitlesWebGame.WebUi.Components;

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
            var previousRoundType =
                JsonConvert.DeserializeObject<GameRoundInfo>(hubMessageModel.AppendedObject.ToString() ??
                                                                           String.Empty).GameRoundsType;

            var deserializer = new RoundInfoDeserializer();
            GameRoundInfo previousRoundInfo = deserializer
                .DeserializeModel(hubMessageModel.AppendedObject.ToString() ?? String.Empty, previousRoundType);
            
            if (previousRoundInfo != null)
            {
                _gameSessionState.UpdatePreviousRound(previousRoundInfo);
            }
        }
    }
}