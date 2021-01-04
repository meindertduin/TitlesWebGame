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

            // Todo: refactor this into a factory
            
            GameRoundInfo previousRoundInfo = null;

            if (previousRoundType == GameRoundsType.MultipleChoiceRound)
            {
                previousRoundInfo = JsonConvert.DeserializeObject<MultipleChoiceRoundInfo>(
                    hubMessageModel.AppendedObject.ToString() ?? String.Empty);
            }

            if (previousRoundType == GameRoundsType.CompetitiveArtistRound)
            {
                previousRoundInfo = JsonConvert.DeserializeObject<CompetitiveArtistRoundInfo>(
                    hubMessageModel.AppendedObject.ToString() ?? String.Empty);
            }
            
            if (previousRoundInfo != null)
            {
                _gameSessionState.UpdatePreviousRound(previousRoundInfo);
            }
        }
    }
}