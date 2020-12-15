using System.Collections.Generic;
using TitlesWebGame.Domain.Entities;

namespace TitlesWebGame.Domain.ViewModels
{
    public class RoundReviewMessageModel
    {
        public List<GameSessionPlayer> GameSessionPlayers { get; set; }
    }
}