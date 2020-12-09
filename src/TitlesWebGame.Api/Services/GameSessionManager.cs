using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TitlesWebGame.Api.Hubs;
using TitlesWebGame.Api.Models;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.Enums;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Services
{
    public class GameSessionManager : IGameSessionManager
    {
        private readonly IHubContext<TitlesGameHub> _titlesGameHub;
        private readonly ITitlesGameHubMessageFactory _titlesGameHubMessageFactory;
        private static ConcurrentDictionary<string, GameSessionState> _gameSessions = new();

        private const int RoomKeyLenght = 6;
        private const int RoundReviewTimeMs = 3000;
        private const int TitlesRoundReviewTimeMs = 2000;

        private Random _random = new();

        public GameSessionManager(IHubContext<TitlesGameHub> titlesGameHub, ITitlesGameHubMessageFactory titlesGameHubMessageFactory)
        {
            _titlesGameHub = titlesGameHub;
            _titlesGameHubMessageFactory = titlesGameHubMessageFactory;
        }
        
        public GameSessionInitViewModel CreateSession(GameSessionPlayer ownerSessionPlayer)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            string roomKey;
            do
            {
                roomKey = new string(Enumerable.Repeat(chars, RoomKeyLenght)
                    .Select(s => s[_random.Next(s.Length)]).ToArray());
            } while (_gameSessions.ContainsKey(roomKey));

            var newGameSession = new GameSessionState()
            {
                RoomKey = roomKey,
                OwnerConnectionId = ownerSessionPlayer.ConnectionId,
            };
            
            newGameSession.AddPlayer(ownerSessionPlayer);
            _titlesGameHub.Groups.AddToGroupAsync(ownerSessionPlayer.ConnectionId, roomKey);

            _gameSessions.TryAdd(roomKey, newGameSession);

            return GetGameSessionState(newGameSession, ownerSessionPlayer);
        }

        private GameSessionInitViewModel GetGameSessionState(GameSessionState gameSessionState, GameSessionPlayer gameSessionPlayer)
        {
            if (gameSessionState != null)
            {
                return new GameSessionInitViewModel()
                {
                    GameSessionPlayers = gameSessionState.GetPlayers(),
                    OwnerConnectionId = gameSessionState.OwnerConnectionId,
                    RoomKey = gameSessionState.RoomKey,
                    CurrentPlayer = gameSessionPlayer,
                };
            }
            return null;
        }
        
        public bool DeleteSession(string roomKey)
        {
            var result = _gameSessions.TryRemove(roomKey, out GameSessionState session);
            return result;
        }

        public void StartSession(string roomKey, string connectionId)
        {
            // Todo: make these values optional
            
            int titleRounds = 1;
            int roundsPerTitle = 1;
            
            var gameSession = _gameSessions.FirstOrDefault(g => g.Key == roomKey).Value;
            
            if (gameSession != null && connectionId == gameSession.OwnerConnectionId)
            {
                Task.Run(() => PlaySessionGame(gameSession, titleRounds, roundsPerTitle));
            }
        }
        
        private async Task PlaySessionGame(GameSessionState gameSessionState, int titleRounds, int roundsPerTitle)
        {
            if (gameSessionState.IsPlaying == false)
            {
                gameSessionState.SetPlayingStatus(true);

                List<TitleCategory> playedRoundCategories = new();
                
                await UpdatePlayersOfGameStated(gameSessionState.RoomKey);

                for (int i = 0; i < roundsPerTitle; i++)
                {
                    // Todo: make this a factory
                    var titleCategory = TitleCategory.Artist;
                    playedRoundCategories.Add(titleCategory);
                    
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
                
                    gameSessionState.SetRoundInfo(loadedRounds);
                
                    while (true)
                    {
                        var newRoundInfo = gameSessionState.GetNextRound();
                        if (newRoundInfo == null)
                        {
                            break;
                        }

                        await UpdatePlayersOfNewRoundInfo(newRoundInfo, gameSessionState.RoomKey);
                        await gameSessionState.PlayNewRound(newRoundInfo);
                        var scores = gameSessionState.GetRoundScores();
                        gameSessionState.AddScores(scores);
                        await UpdatePlayersOfSessionState(gameSessionState.RoomKey ,newRoundInfo, gameSessionState.GetPlayers());
                        await Task.Delay(RoundReviewTimeMs);
                    }
                    
                    await UpdatePlayersOfTitlesRoundEnded(gameSessionState);
                    await Task.Delay(TitlesRoundReviewTimeMs);
                    gameSessionState.EndTitlesRound(titleCategory);
                }
                
                gameSessionState.SetPlayingStatus(false);
                await UpdatePlayersOfEndGame(gameSessionState);
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
        
        private Task UpdatePlayersOfNewRoundInfo(GameRoundInfo gameRoundInfo, string roomKey)
        {
            GameRoundInfoViewModel newGameRoundInfoVm = null;
            
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
        
        private Task UpdatePlayersOfSessionState(string roomKey, GameRoundInfo currentRoundInfo, List<GameSessionPlayer> gameSessionPlayers)
        {
            var previousRoundInfo = new SessionStateUpdateViewModel()
            {
                GameSessionPlayers = gameSessionPlayers,
                PreviousRoundInfo = currentRoundInfo,
            };
            
            return _titlesGameHub.Clients.Group(roomKey).SendAsync("ServerMessageUpdate", 
                _titlesGameHubMessageFactory.CreatePreviousRoundInfoMessage(previousRoundInfo));
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
        
        public GameSessionInitViewModel JoinSession(string roomKey, GameSessionPlayer gameSessionPlayer)
        {
            var gameSession = _gameSessions.FirstOrDefault(g => g.Key == roomKey).Value;
            if (gameSession != null)
            {
                var addPlayerSuccessful = gameSession.AddPlayer(gameSessionPlayer);
                if (addPlayerSuccessful)
                {
                    return GetGameSessionState(gameSession, gameSessionPlayer);
                }
            }
            
            return null;
        }

        public bool AddAnswer(string roomKey, GameRoundAnswer gameRoundAnswer)
        {
            var gameSession = _gameSessions.FirstOrDefault(g => g.Key == roomKey).Value;
            return gameSession.AddAnswer(gameRoundAnswer);
        }
    }
}