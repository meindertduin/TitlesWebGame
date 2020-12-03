using System.Collections.Generic;
using TitlesWebGame.Api.Models;

namespace TitlesWebGame.Api.Hubs
{
    public class SessionStateUpdateViewModel
    {
        public List<GameSessionPlayer> GameSessionPlayers { get; set; }
        public GameRoundInfo PreviousRoundInfo { get; set; }
    }
    
}