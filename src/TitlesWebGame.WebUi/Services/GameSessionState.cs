using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TitlesWebGame.Domain.Entities;

namespace TitlesWebGame.WebUi.Services
{
    public class GameSessionState
    {
        public GameSessionPlayer GameSessionPlayer { get; private set; }
        public string RoomKey { get; init; }
        public List<GameSessionPlayer> Players { get; private set; }
        
        public void SetPlayerStates(List<GameSessionPlayer> playerStates)
        {
            Players = playerStates;
        }

        public void AddPlayer(GameSessionPlayer player)
        {
            Players.Add(player);
        }

        public void RemovePlayer(string connectionId)
        {
            var leavingPlayer = Players.FirstOrDefault(x => x.ConnectionId == connectionId);
            Players.Remove(leavingPlayer);
        }
    }   
}