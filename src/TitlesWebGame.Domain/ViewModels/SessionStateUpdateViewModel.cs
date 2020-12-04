using System.Collections.Generic;
using TitlesWebGame.Domain.Entities;

namespace TitlesWebGame.Domain.ViewModels
{
    public class SessionStateUpdateViewModel
    {
        public List<GameSessionPlayer> GameSessionPlayers { get; set; }
        public GameRoundInfo PreviousRoundInfo { get; set; }
    }
}