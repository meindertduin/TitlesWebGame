using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using TitlesWebGame.Api.Hubs;
using TitlesWebGame.Api.Models;

namespace TitlesWebGame.Api.Services
{
    public class GameSessionManager : IGameConnectionGroupsManager
    {
        private readonly IHubContext<TitlesGameHub> _titlesGameHub;
        private static ConcurrentDictionary<string, GameSession> _gameSessions = new();

        private const int RoomKeyLenght = 6;

        private Random _random = new Random();

        public GameSessionManager(IHubContext<TitlesGameHub> titlesGameHub)
        {
            _titlesGameHub = titlesGameHub;
        }
        
        public string CreateSession()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            string roomKey;
            do
            {
                roomKey = new string(Enumerable.Repeat(chars, RoomKeyLenght)
                    .Select(s => s[_random.Next(s.Length)]).ToArray());
            } while (_gameSessions.ContainsKey(roomKey) == false);

            _gameSessions.TryAdd(roomKey, new GameSession(_titlesGameHub)
            {
                RoomKey = roomKey,
            });

            return roomKey;
        }

        public bool DeleteSession(string roomKey)
        {
            var result = _gameSessions.TryRemove(roomKey, out GameSession session);
            return result;
        }
    }
}