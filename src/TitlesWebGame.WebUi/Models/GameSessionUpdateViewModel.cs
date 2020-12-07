using System.Collections.Generic;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Models
{
    public class GameSessionUpdateViewModel
    {
        public string OwnerConnectionId { get; set; }
        public bool IsPlaying { get; set; }
        public bool HasEnded { get; set; }
        public string RoomKey { get; set; }
        public List<GameSessionPlayer> GameSessionPlayers { get; set; }
        public GameRoundInfoViewModel NextRoundInfo { get; set; }
        public GameRoundInfo PreviousRoundInfo { get; set; }
    }
}