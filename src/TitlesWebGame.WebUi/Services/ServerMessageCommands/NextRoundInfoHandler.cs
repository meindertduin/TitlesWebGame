using System;
using Newtonsoft.Json;
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
            if (nextRoundType == GameRoundsType.MultipleChoiceRound)
            {
                nextRoundInfo = JsonConvert.DeserializeObject<MultipleChoiceRoundInfoViewModel>(
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