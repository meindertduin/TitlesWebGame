using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TitlesWebGame.Api.Extensions;
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

                GameRoundInfoViewModel newGameRoundInfoVm = null;

                newGameRoundInfoVm = ProjectToRoundInfoViewModel(newRoundInfo, newGameRoundInfoVm);
                
                await UpdatePlayersOfNewRoundInfo(newGameRoundInfoVm, gameSessionState.RoomKey);
                await PlayGameRound(gameSessionState, newRoundInfo);
                await UpdatePlayersOfPreviousRoundInfo(gameSessionState.RoomKey, newRoundInfo);
                await UpdatePlayersOfRoundReview(gameSessionState.RoomKey, gameSessionState.GetPlayers());
                await Task.Delay(RoundReviewTimeMs);
            }
        }
        
        private Task UpdatePlayersOfNewRoundInfo(GameRoundInfoViewModel gameRoundInfo, string roomKey)
        {
            if (gameRoundInfo != null)
            {
                return _titlesGameHub.Clients.Group(roomKey).SendAsync("ServerMessageUpdate", 
                    _titlesGameHubMessageFactory.CreateNextRoundInfoMessage(gameRoundInfo));
            }
            else
            {
                throw new ArgumentNullException(nameof(gameRoundInfo));
            }
        }
        private async Task PlayGameRound(GameSessionState gameSessionState, GameRoundInfo newRoundInfo)
        {
            if (newRoundInfo is CompetitiveArtistRoundInfo competitiveArtistRound)
            {
                var players = gameSessionState.GetPlayers();
                var roundStatement = "Draw a car with only triangles";

                
                // make match ups for everyone
                List<(string PlayerOne, string PlayerTwo, string roundStatement)> matchUps = new();

                if (players.Count % 2 != 0)
                {
                    gameSessionState.AddBot();
                }
                
                players.Shuffle();
                
                for (int i = 0; i < players.Count; i+=2)
                {
                    // get random round statement
                    matchUps.Add((players[i].ConnectionId, players[i + 1].ConnectionId, roundStatement));
                }
                
                // notify players of painting round info and get them painting!
                var paintingRoundInfoVm = new CanvasPaintingRoundInfoViewModel()
                {
                    RoundStatement = roundStatement,
                    RoundTimeMs = competitiveArtistRound.PaintingRoundTimeMs,
                    GameRoundsType = GameRoundsType.CanvasPaintingRound,
                    TitleCategory = competitiveArtistRound.TitleCategory,
                };

                // Todo: change this into individual sending of viewModels depending on roundStatement and matchUps
                await UpdatePlayersOfNewRoundInfo(paintingRoundInfoVm, gameSessionState.RoomKey);
                
                // play the painting round first
                await gameSessionState.PlayNewRound(new CanvasPaintingRound(competitiveArtistRound.PaintingRoundTimeMs));

                // get the painting answers
                var answerData = gameSessionState.GetRoundAnswersData();
                
                // play voting rounds
                for (int i = 0; i < matchUps.Count; i++)
                {
                    var answerDataPlayerOne = answerData.FirstOrDefault(x => x.ConnectionId == matchUps[i].PlayerOne);
                    var answerDataPlayerTwo = answerData.FirstOrDefault(x => x.ConnectionId == matchUps[i].PlayerTwo);
                    
                    // give players voting round info
                    var votingRoundInfoVm = new CompetitiveArtistVotingRoundInfoViewModel()
                    {
                        RoundTimeMs = competitiveArtistRound.VotingRoundTimeMs,
                        RoundStatement = roundStatement,
                        GameRoundsType = GameRoundsType.CompetitiveArtistVotingRound,
                        Choices = new[] {answerDataPlayerOne, answerDataPlayerTwo},
                    };

                    await UpdatePlayersOfNewRoundInfo(votingRoundInfoVm, gameSessionState.RoomKey);
                    
                    // play new voting round
                    await gameSessionState.PlayNewRound(new CompetitiveArtistVotingRound(competitiveArtistRound.VotingRoundTimeMs, competitiveArtistRound.RewardPoints));
                    var scores = gameSessionState.GetRoundScores();
                    gameSessionState.AddScores(scores);
                    
                    // voting round review
                    await UpdatePlayersOfPreviousRoundInfo(gameSessionState.RoomKey, newRoundInfo);
                    await UpdatePlayersOfRoundReview(gameSessionState.RoomKey, gameSessionState.GetPlayers());
                }
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