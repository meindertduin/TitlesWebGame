using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TitlesWebGame.Api.Hubs;
using TitlesWebGame.Api.Models;

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
        
        public string CreateSession(GameSessionPlayer ownerSessionPlayer)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            string roomKey;
            do
            {
                roomKey = new string(Enumerable.Repeat(chars, RoomKeyLenght)
                    .Select(s => s[_random.Next(s.Length)]).ToArray());
            } while (_gameSessions.ContainsKey(roomKey) == false);

            var newGameSession = new GameSession(_titlesGameHub)
            {
                RoomKey = roomKey,
                OwnerConnectionId = ownerSessionPlayer.ConnectionId,
            };
            newGameSession.AddPlayer(ownerSessionPlayer);
            
            _gameSessions.TryAdd(roomKey, newGameSession);

            return roomKey;
        }
        
        public bool DeleteSession(string roomKey)
        {
            var result = _gameSessions.TryRemove(roomKey, out GameSession session);
            return result;
        }

        public void StartSession(string roomKey, string connectionId)
        {
            var gameSession = _gameSessions.FirstOrDefault(g => g.Key == roomKey).Value;
            if (gameSession != null)
            {
                Task.Run(() => gameSession.PlayGame(connectionId));
            }
        }

        public bool JoinSession(string roomKey, GameSessionPlayer gameSessionPlayer)
        {
            var gameSession = _gameSessions.FirstOrDefault(g => g.Key == roomKey).Value;
            return gameSession.AddPlayer(gameSessionPlayer);
        }

        public bool AddAnswer(string roomKey, GameRoundAnswer gameRoundAnswer)
        {
            var gameSession = _gameSessions.FirstOrDefault(g => g.Key == roomKey).Value;
            return gameSession.AddAnswer(gameRoundAnswer);
        }
    }
}