using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private static ConcurrentDictionary<string, GameSession> _gameSessions = new();

        private const int RoomKeyLenght = 6;

        private Random _random = new();

        public GameSessionManager(IHubContext<TitlesGameHub> titlesGameHub)
        {
            _titlesGameHub = titlesGameHub;
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

            var newGameSession = new GameSession()
            {
                RoomKey = roomKey,
                OwnerConnectionId = ownerSessionPlayer.ConnectionId,
            };
            
            newGameSession.AddPlayer(ownerSessionPlayer);
            _titlesGameHub.Groups.AddToGroupAsync(ownerSessionPlayer.ConnectionId, roomKey);

            _gameSessions.TryAdd(roomKey, newGameSession);

            return GetGameSessionState(newGameSession, ownerSessionPlayer);
        }

        private GameSessionInitViewModel GetGameSessionState(GameSession gameSession, GameSessionPlayer gameSessionPlayer)
        {
            if (gameSession != null)
            {
                return new GameSessionInitViewModel()
                {
                    GameSessionPlayers = gameSession.GetPlayers(),
                    OwnerConnectionId = gameSession.OwnerConnectionId,
                    RoomKey = gameSession.RoomKey,
                    CurrentPlayer = gameSessionPlayer,
                };
            }

            return null;
        }
        
        public bool DeleteSession(string roomKey)
        {
            var result = _gameSessions.TryRemove(roomKey, out GameSession session);
            return result;
        }

        public void StartSession(string roomKey, string connectionId)
        {
            var gameSession = _gameSessions.FirstOrDefault(g => g.Key == roomKey).Value;
            
            if (gameSession != null && connectionId == gameSession.OwnerConnectionId)
            {
                Task.Run(() => PlaySessionGame(gameSession));
            }
        }

        private async Task PlaySessionGame(GameSession gameSession)
        {
            if (gameSession.IsPlaying == false)
            {
                gameSession.SetPlayingStatus(true);
                
                // Todo: make this a factory
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
            
                gameSession.SetRoundInfo(loadedRounds);
                
                await UpdatePlayersOfGameStated(gameSession.RoomKey);
                
                while (true)
                {
                    var newRoundInfo = gameSession.GetNextRound();
                    if (newRoundInfo == null)
                    {
                        break;
                    }

                    await UpdatePlayersOfNewRoundInfo(newRoundInfo, gameSession.RoomKey);
                    await gameSession.PlayNewRound(newRoundInfo);
                    var scores = gameSession.GetRoundScores();
                    gameSession.AddScores(scores);
                    await UpdatePlayersOfSessionState(gameSession.RoomKey ,newRoundInfo, gameSession.GetPlayers());
                }
                gameSession.SetPlayingStatus(false);
                await UpdatePlayersOfEndGame(gameSession);
            }
        }

        private Task UpdatePlayersOfGameStated(string roomKey)
        {
            int startingAfterDelay = 1;

            return _titlesGameHub.Clients.Group(roomKey).SendAsync("ServerMessageUpdate",
                new TitlesGameHubMessageModel()
                {
                    Error = false,
                    Message = "Game starting",
                    AppendedObject = startingAfterDelay,
                    MessageType = GameHubMessageType.SessionStarted,
                });
        }

        private Task UpdatePlayersOfEndGame(GameSession gameSession)
        {
            var endGameResults = new TitlesGameEndGameResults()
            {
                GameSessionPlayers = gameSession.GetPlayers(),
            };

            return _titlesGameHub.Clients.Group(gameSession.RoomKey).SendAsync("EndGameResultsUpdate", endGameResults);
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
                return _titlesGameHub.Clients.Group(roomKey).SendAsync("ServerMessageUpdate", new TitlesGameHubMessageModel()
                {
                    Message = "Next round info",
                    MessageType = GameHubMessageType.NextRoundInfo,
                    AppendedObject = newGameRoundInfoVm,
                    Error = false,
                });
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
            
            return _titlesGameHub.Clients.Group(roomKey).SendAsync("ServerMessageUpdate", new TitlesGameHubMessageModel()
            {
                Error = false,
                Message = "Previous round update and state loaded",
                MessageType = GameHubMessageType.PreviousRoundInfo,
                AppendedObject = previousRoundInfo,
            });
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