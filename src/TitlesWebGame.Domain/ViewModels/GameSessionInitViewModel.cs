using System.Collections.Generic;
using TitlesWebGame.Domain.Entities;

namespace TitlesWebGame.Domain.ViewModels
{
    public class GameSessionInitViewModel
    {
        public string RoomKey { get; set; }
        public string OwnerConnectionId { get; set; }
        public List<GameSessionPlayer> GameSessionPlayers { get; set; }

        public GameSessionPlayer CurrentPlayer { get; set; }
    }
}