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

            GameRoundInfoViewModel nextRoundInfo = null;
            
            // Todo: Set this into a factory class

            if (nextRoundType == GameRoundsType.MultipleChoiceRound)
            {
                nextRoundInfo = JsonConvert.DeserializeObject<MultipleChoiceRoundInfoViewModel>(
                    hubMessageModel.AppendedObject.ToString() ?? String.Empty);
            }
            else if(nextRoundType == GameRoundsType.CanvasPaintingRound)
            {
                nextRoundInfo =
                    JsonConvert.DeserializeObject<CanvasPaintingRoundInfoViewModel>(
                        hubMessageModel.AppendedObject.ToString() ?? String.Empty);
            }
            else if (nextRoundType == GameRoundsType.CompetitiveArtistVotingRound)
            {
                nextRoundInfo = JsonConvert.DeserializeObject<CompetitiveArtistVotingRoundInfoViewModel>(
                    hubMessageModel.AppendedObject.ToString() ?? String.Empty);
            }
            else if (nextRoundType == GameRoundsType.CompetitiveArtistReviewRound)
            {
                nextRoundInfo = JsonConvert.DeserializeObject<CompetitiveArtistReviewRoundInfoViewModel>(
                    hubMessageModel.AppendedObject.ToString() ?? String.Empty);
            }
            
            if (nextRoundInfo != null)
            {
                _gameSessionState.SetNextRoundInfo(nextRoundInfo);
                Console.WriteLine("next round info loaded");
            }
        }
    }
}