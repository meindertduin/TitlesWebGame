using System;
using System.Threading.Tasks;
using TitlesWebGame.Api.Models;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Services
{
    public class MultipleChoiceRoundController : IGameRoundController
    {
        private readonly IGameSessionClientMessageService _clientMessageService;

        public MultipleChoiceRoundController(IGameSessionClientMessageService clientMessageService)
        {
            _clientMessageService = clientMessageService;
        }

        public async Task PlayGameRound(GameSessionState gameSessionState, GameRoundInfo gameRoundInfo)
        {
            var roundInfo = gameRoundInfo as MultipleChoiceRoundInfo;
            if (roundInfo == null)
            {
                throw new InvalidCastException(nameof(roundInfo));
            }
            
            var newGameRoundInfoVm = ProjectToRoundInfoViewModel(gameRoundInfo);
            await _clientMessageService.UpdatePlayersOfNewRoundInfo(gameSessionState.RoomKey, newGameRoundInfoVm);
            
            await gameSessionState.PlayNewRound(new MultipleChoiceGameRound(roundInfo.Answer, roundInfo.RewardPoints, roundInfo.RoundTimeMs));
            var scores = gameSessionState.GetRoundScores();
            gameSessionState.AddScores(scores);
        }
        
        private GameRoundInfoViewModel ProjectToRoundInfoViewModel(GameRoundInfo gameRoundInfo)
        {
            if (gameRoundInfo is MultipleChoiceRoundInfo multipleChoiceRoundInfo)
            {
                return new MultipleChoiceRoundInfoViewModel()
                {
                    RoundTimeMs = multipleChoiceRoundInfo.RoundTimeMs,
                    RewardPoints = multipleChoiceRoundInfo.RewardPoints,
                    Choices = multipleChoiceRoundInfo.Choices.Split(','),
                    RoundStatement = multipleChoiceRoundInfo.RoundStatement,
                    GameRoundsType = multipleChoiceRoundInfo.GameRoundsType,
                    TitleCategory = multipleChoiceRoundInfo.TitleCategory,
                };
            }

            return null;
        }
    }
}