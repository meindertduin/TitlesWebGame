using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TitlesWebGame.Api.Hubs;
using TitlesWebGame.Api.Models;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.Enums;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Services
{
    public class GameSessionControllerService : IGameSessionControllerService
    {
        private readonly IHubContext<TitlesGameHub> _titlesGameHub;
        private readonly ITitlesGameHubMessageFactory _titlesGameHubMessageFactory;
        private const int RoundReviewTimeMs = 3000;
        private const int TitlesRoundReviewTimeMs = 2000;
        
        public GameSessionControllerService(IHubContext<TitlesGameHub> titlesGameHub, ITitlesGameHubMessageFactory titlesGameHubMessageFactory)
        {
            _titlesGameHub = titlesGameHub;
            _titlesGameHubMessageFactory = titlesGameHubMessageFactory;
        }
        
        public async Task PlaySessionGame(GameSessionState gameSessionState, GameSessionStartOptions gameSessionStartOptions)
        {
            if (gameSessionState.IsPlaying == false)
            {
                gameSessionState.SetPlayingStatus(true);
                await UpdatePlayersOfGameStated(gameSessionState.RoomKey);
                
                await PlayTitleRounds(gameSessionState, gameSessionStartOptions);
                
                gameSessionState.SetPlayingStatus(false);
                await UpdatePlayersOfEndGame(gameSessionState);
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
                await UpdatePlayersOfTitlesRoundEnded(gameSessionState);
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

                await UpdatePlayersOfNewRoundInfo(newRoundInfo, gameSessionState.RoomKey);
                await PlayGameRound(gameSessionState, newRoundInfo);
                await UpdatePlayersOfPreviousRoundInfo(gameSessionState.RoomKey, newRoundInfo);
                await UpdatePlayersOfRoundReview(gameSessionState.RoomKey, gameSessionState.GetPlayers());
                await Task.Delay(RoundReviewTimeMs);
            }
        }
        
        private Task UpdatePlayersOfNewRoundInfo(GameRoundInfo gameRoundInfo, string roomKey)
        {
            GameRoundInfoViewModel newGameRoundInfoVm = null;

            newGameRoundInfoVm = ProjectToRoundInfoViewModel(gameRoundInfo, newGameRoundInfoVm);

            if (newGameRoundInfoVm != null)
            {
                return _titlesGameHub.Clients.Group(roomKey).SendAsync("ServerMessageUpdate", 
                    _titlesGameHubMessageFactory.CreateNextRoundInfoMessage(newGameRoundInfoVm));
            }
            else
            {
                throw new ArgumentNullException(nameof(newGameRoundInfoVm));
            }
        }
        private async Task PlayGameRound(GameSessionState gameSessionState, GameRoundInfo newRoundInfo)
        {
            if (newRoundInfo is CompetitiveArtistRoundInfo competitiveArtistRound)
            {
                // play the painting round first
                await gameSessionState.PlayNewRound(new CanvasPaintingRound(competitiveArtistRound.PaintingRoundTimeMs));
                // get the painting answers
                var answerData = gameSessionState.GetRoundAnswersData();
                // play voting rounds
                var players = gameSessionState.GetPlayers();
                
                var roundsAmount = players.Count % 2 == 0 ? players.Count / 2 : players.Count / 2 + 1;
                
                for (int i = 0; i < roundsAmount; i++)
                {
                    // give players voting round info
                    
                    // play new voting round
                    await gameSessionState.PlayNewRound(new CompetitiveArtistVotingRound(competitiveArtistRound.VotingRoundTimeMs));
                    
                    // voting round review
                    
                }
                
                // get winner
            }

            if (newRoundInfo is MultipleChoiceRoundInfo multipleChoiceRoundInfo)
            {
                await gameSessionState.PlayNewRound(new MultipleChoiceGameRound(multipleChoiceRoundInfo.Answer.ToString(), multipleChoiceRoundInfo.RewardPoints, multipleChoiceRoundInfo.RoundTimeMs));
                var scores = gameSessionState.GetRoundScores();
                gameSessionState.AddScores(scores);
            }
        }
        
        private Task UpdatePlayersOfGameStated(string roomKey)
        {
            int startingAfterDelay = 1;

            return _titlesGameHub.Clients.Group(roomKey).SendAsync("ServerMessageUpdate",
                _titlesGameHubMessageFactory.CreateSessionStartedMessage(startingAfterDelay));
        }
        
        private Task UpdatePlayersOfEndGame(GameSessionState gameSessionState)
        {
            var endGameResults = new TitlesGameEndSessionResults()
            {
                GameSessionPlayers = gameSessionState.GetPlayers(),
            };

            return _titlesGameHub.Clients.Group(gameSessionState.RoomKey).SendAsync("ServerMessageUpdate",
                _titlesGameHubMessageFactory.CreateEndSessionMessage(endGameResults));
        }

        private GameRoundInfoViewModel ProjectToRoundInfoViewModel(GameRoundInfo gameRoundInfo,
            GameRoundInfoViewModel newGameRoundInfoVm)
        {
            if (gameRoundInfo is MultipleChoiceRoundInfo multipleChoiceRoundInfo)
            {
                newGameRoundInfoVm = new MultipleChoiceRoundInfoViewModel()
                {
                    RoundTimeMs = multipleChoiceRoundInfo.RoundTimeMs,
                    RewardPoints = multipleChoiceRoundInfo.RewardPoints,
                    Choices = multipleChoiceRoundInfo.Choices,
                    RoundStatement = multipleChoiceRoundInfo.RoundStatement,
                    GameRoundsType = multipleChoiceRoundInfo.GameRoundsType,
                    TitleCategory = multipleChoiceRoundInfo.TitleCategory,
                };
            }

            return newGameRoundInfoVm;
        }

        private Task UpdatePlayersOfPreviousRoundInfo(string roomKey, GameRoundInfo previousRoundInfo)
        {
            return _titlesGameHub.Clients.Group(roomKey).SendAsync("ServerMessageUpdate",
                _titlesGameHubMessageFactory.CreatePreviousRoundInfoMessage(previousRoundInfo));
        }
        
        private Task UpdatePlayersOfRoundReview(string roomKey, List<GameSessionPlayer> gameSessionPlayers)
        {
            var previousRoundInfo = new RoundReviewMessageModel()
            {
                GameSessionPlayers = gameSessionPlayers,
            };
            
            return _titlesGameHub.Clients.Group(roomKey).SendAsync("ServerMessageUpdate", 
                _titlesGameHubMessageFactory.CreateRoundReviewMessage(previousRoundInfo));
        }

        private Task UpdatePlayersOfTitlesRoundEnded(GameSessionState gameSessionState)
        {
            var titlesGameRoundResults = new TitlesRoundResults()
            {
                Players = gameSessionState.GetPlayers(),
            };
            
            return _titlesGameHub.Clients.Group(gameSessionState.RoomKey).SendAsync("ServerMessageUpdate", 
                _titlesGameHubMessageFactory.CreateEndTitlesRoundMessage(titlesGameRoundResults));
        }
    }
}