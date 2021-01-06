using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TitlesWebGame.Api.Infrastructure.Repositories;
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
        private readonly IGameRoundInfoRepository _gameRoundInfoRepository;
        private const int RoundReviewTimeMs = 3000;
        private const int TitlesRoundReviewTimeMs = 2000;
        private IGameRoundController _currentRoundController;
        
        public GameSessionControllerService(IServiceProvider serviceProvider, IGameSessionClientMessageService clientMessageService, 
            IGameRoundInfoRepository gameRoundInfoRepository)
        {
            _serviceProvider = serviceProvider;
            _clientMessageService = clientMessageService;
            _gameRoundInfoRepository = gameRoundInfoRepository;
        }
        
        public async Task PlaySessionGame(GameSessionState gameSessionState, GameSessionStartOptions gameSessionStartOptions)
        {
            if (gameSessionState.IsPlaying == false)
            {
                gameSessionState.SetPlayingStatus(true);
                await _clientMessageService.UpdatePlayersOfGameStarted(gameSessionState.RoomKey);
                
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
                // Todo: show clients a loading page of the title rounds with a description
                
                var titleCategory = TitleCategory.Artist;
                playedRoundCategories.Add(titleCategory);

                var loadedRounds = await GetTitleRoundRounds(gameSessionStartOptions.RoundsPerTitleAmount);

                gameSessionState.SetRoundInfo(loadedRounds);

                await PlayGameRounds(gameSessionState);
                
                gameSessionState.EndTitlesRound(titleCategory);
                await _clientMessageService.UpdatePlayersOfTitlesRoundEnded(gameSessionState.RoomKey, gameSessionState.GetPlayers());
                await Task.Delay(TitlesRoundReviewTimeMs);
                gameSessionState.ResetPlayerPoints();
            }
        }

        private async Task<List<GameRoundInfo>> GetTitleRoundRounds(int amount)
        {
            // gets title rounds the repository
            var gameRound = await _gameRoundInfoRepository.GetAsync(1);
            return new List<GameRoundInfo>() { gameRound };
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
    }
}