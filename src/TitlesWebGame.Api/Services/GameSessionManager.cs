using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TitlesWebGame.Api.Hubs;
using TitlesWebGame.Api.Models;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Services
{
    public class GameSessionManager : IGameSessionManager
    {
        private readonly IHubContext<TitlesGameHub> _titlesGameHub;
        private readonly IGameSessionControllerService _gameSessionControllerService;
        private readonly ITitlesGameHubMessageFactory _titlesGameHubMessageFactory;
        private readonly IGameSessionClientMessageService _gameSessionClientMessageService;

        private static ConcurrentDictionary<string, GameSessionState> _gameSessions = new();

        private const int RoomKeyLenght = 6;


        private Random _random = new();

        public GameSessionManager(IHubContext<TitlesGameHub> titlesGameHub, IGameSessionControllerService gameSessionControllerService,
            ITitlesGameHubMessageFactory titlesGameHubMessageFactory, IGameSessionClientMessageService gameSessionClientMessageService)
        {
            _titlesGameHub = titlesGameHub;
            _gameSessionControllerService = gameSessionControllerService;
            _titlesGameHubMessageFactory = titlesGameHubMessageFactory;
            _gameSessionClientMessageService = gameSessionClientMessageService;
        }
        
        public static void CleanUpEmptySessions()
        {
            foreach (var gameSessionState in _gameSessions)
            {
                if (gameSessionState.Value.GetPlayersCount() <= 0)
                {
                    _gameSessions.Remove(gameSessionState.Key, out var value);
                }
            }
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

        public void StartSession(string roomKey, string connectionId, GameSessionStartOptions gameSessionStartOptions)
        {
            var gameSession = _gameSessions.FirstOrDefault(g => g.Key == roomKey).Value;
            
            if (gameSession != null && connectionId == gameSession.OwnerConnectionId)
            {
                Task.Run(() => _gameSessionControllerService.PlaySessionGame(gameSession, gameSessionStartOptions));
            }
        }

        public bool PlayAgain(string roomKey, string connectionId)
        {
            var getSessionSucceeded =  _gameSessions.TryGetValue(roomKey, out GameSessionState gameSession);

            if (getSessionSucceeded)
            {
                if (gameSession.OwnerConnectionId == connectionId)
                {
                    return true;
                }
            }

            return false;
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

        public bool LeaveSession(string roomKey, string connectionId)
        {
            var gameSession = _gameSessions.FirstOrDefault(g => g.Key == roomKey).Value;
            if (gameSession != null)
            { 
                var removeResult = gameSession.RemovePlayer(connectionId, out string newOwnerConId);
                if (String.IsNullOrEmpty(newOwnerConId) == false)
                {
                    _gameSessionClientMessageService.UpdatePlayersOfNewGroupOwner(roomKey, connectionId);
                }

                return removeResult;
            }

            return false;
        }

        public bool AddAnswer(string roomKey, GameRoundAnswer gameRoundAnswer)
        {
            var gameSession = _gameSessions.FirstOrDefault(g => g.Key == roomKey).Value;
            return gameSession.AddAnswer(gameRoundAnswer);
        }

        public List<GameRoundAnswer> GetGameRoundAnswers(string roomKey)
        {
            var gameSession = _gameSessions.FirstOrDefault(g => g.Key == roomKey).Value;
            return gameSession.GetRoundAnswers();
        }

        public List<GameRoundAnswer> GetGameRoundAnswers(string roomKey, string[] connections)
        {
            var gameSession = _gameSessions.FirstOrDefault(g => g.Key == roomKey).Value;
            return gameSession.GetRoundAnswers(connections);
        }
    }
}