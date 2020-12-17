using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TitlesWebGame.Api.Models;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.Enums;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Services
{
    public class GameSessionControllerService : IGameSessionControllerService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IGameSessionClientMessageService _clientMessageService;
        private const int RoundReviewTimeMs = 3000;
        private const int TitlesRoundReviewTimeMs = 2000;
        private IGameRoundController _currentRoundController;
        
        public GameSessionControllerService(IServiceProvider serviceProvider, IGameSessionClientMessageService clientMessageService)
        {
            _serviceProvider = serviceProvider;
            _clientMessageService = clientMessageService;
        }
        
        public async Task PlaySessionGame(GameSessionState gameSessionState, GameSessionStartOptions gameSessionStartOptions)
        {
            if (gameSessionState.IsPlaying == false)
            {
                gameSessionState.SetPlayingStatus(true);
                await _clientMessageService.UpdatePlayersOfGameStated(gameSessionState.RoomKey);
                
                await PlayTitleRounds(gameSessionState, gameSessionStartOptions);
                
                gameSessionState.SetPlayingStatus(false);
                await _clientMessageService.UpdatePlayersOfEndGame(gameSessionState.RoomKey, gameSessionState.GetPlayers());
            }
        }

        private async Task PlayTitleRounds(GameSessionState gameSessionState, GameSessionStartOptions gameSessionStartOptions)
        {
            List<TitleCategory> playedRoundCategories = new();

            for (int i = 0; i < gameSessionStartOptions.TitleRoundsAmount; i++)
            {
                var titleCategory = TitleCategory.Scientist;
                playedRoundCategories.Add(titleCategory);

                var loadedRounds = GetTitleRoundRounds(gameSessionStartOptions.RoundsPerTitleAmount);

                gameSessionState.SetRoundInfo(loadedRounds);

                await PlayGameRounds(gameSessionState);
                
                gameSessionState.EndTitlesRound(titleCategory);
                await _clientMessageService.UpdatePlayersOfTitlesRoundEnded(gameSessionState.RoomKey, gameSessionState.GetPlayers());
                await Task.Delay(TitlesRoundReviewTimeMs);
                gameSessionState.ResetPlayerPoints();
            }
        }

        private List<GameRoundInfo> GetTitleRoundRounds(int amount)
        {
            var loadedRounds = new List<GameRoundInfo>()
            {
                new MultipleChoiceRoundInfo()
                {
                    Answer = 1.ToString(),
                    Choices = new[] {"bear", "zebra", "giraffe", "crocodile"},
                    RewardPoints = 500,
                    GameRoundsType = GameRoundsType.MultipleChoiceRound,
                    RoundStatement = "What animal is primarily known for having stripes",
                    RoundTimeMs = 3000,
                    TitleCategory = TitleCategory.Scientist,
                },
            };
            return loadedRounds;
        }

        private async Task PlayGameRounds(GameSessionState gameSessionState)
        {
            while (true)
            {
                var newRoundInfo = gameSessionState.GetNextRound();
                if (newRoundInfo == null)
                {
                    break;
                }

                GameRoundInfoViewModel newGameRoundInfoVm = null;

                newGameRoundInfoVm = ProjectToRoundInfoViewModel(newRoundInfo);
                
                await _clientMessageService.UpdatePlayersOfNewRoundInfo(gameSessionState.RoomKey, newGameRoundInfoVm);
                await PlayGameRound(gameSessionState, newRoundInfo);
                await _clientMessageService.UpdatePlayersOfPreviousRoundInfo(gameSessionState.RoomKey, newRoundInfo);
                await _clientMessageService.UpdatePlayersOfRoundReview(gameSessionState.RoomKey, gameSessionState.GetPlayers());
                await Task.Delay(RoundReviewTimeMs);
            }
        }
        
        
        private async Task PlayGameRound(GameSessionState gameSessionState, GameRoundInfo newRoundInfo)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                if (newRoundInfo.GameRoundsType == GameRoundsType.CompetitiveArtistRound)
                {
                    _currentRoundController =
                        scope.ServiceProvider.GetRequiredService<CompetitiveArtistRoundController>();
                }
                else if (newRoundInfo.GameRoundsType == GameRoundsType.MultipleChoiceRound)
                {
                    _currentRoundController = scope.ServiceProvider.GetRequiredService<MultipleChoiceRoundController>();
                }
            }

            if (_currentRoundController != null)
            {
                await _currentRoundController.PlayGameRound(gameSessionState, newRoundInfo);
            }
            else
            {
                throw new NullReferenceException(nameof(_currentRoundController));
            }
        }
        
        private GameRoundInfoViewModel ProjectToRoundInfoViewModel(GameRoundInfo gameRoundInfo)
        {
            if (gameRoundInfo is MultipleChoiceRoundInfo multipleChoiceRoundInfo)
            {
                return new MultipleChoiceRoundInfoViewModel()
                {
                    RoundTimeMs = multipleChoiceRoundInfo.RoundTimeMs,
                    RewardPoints = multipleChoiceRoundInfo.RewardPoints,
                    Choices = multipleChoiceRoundInfo.Choices,
                    RoundStatement = multipleChoiceRoundInfo.RoundStatement,
                    GameRoundsType = multipleChoiceRoundInfo.GameRoundsType,
                    TitleCategory = multipleChoiceRoundInfo.TitleCategory,
                };
            }

            return null;
        }
    }
}