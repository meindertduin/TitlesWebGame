using System;
using System.Collections.Concurrent;
using System.Linq;
using TitlesWebGame.Api.Models;

namespace TitlesWebGame.Api.Services
{
    public class GameConnectionGroupsManager : IGameConnectionGroupsManager
    {
        private static ConcurrentDictionary<string, GameSession> _gameSessions = new();

        private const int RoomKeyLenght = 6;

        private Random _random = new Random();
        
        public string CreateSession()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            string roomKey;
            do
            {
                roomKey = new string(Enumerable.Repeat(chars, RoomKeyLenght)
                    .Select(s => s[_random.Next(s.Length)]).ToArray());
            } while (_gameSessions.ContainsKey(roomKey) == false);

            _gameSessions.TryAdd(roomKey, new GameSession()
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