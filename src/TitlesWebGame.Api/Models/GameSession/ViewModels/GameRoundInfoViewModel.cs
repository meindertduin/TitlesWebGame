using TitlesWebGame.Api.Models;

namespace TitlesWebGame.Api.Hubs
{
    public abstract class GameRoundInfoViewModel
    {
        public string RoundStatement { get; set; }
        public GameRoundsType GameRoundsType { get; set; }
        public TitleCategories TitleCategory { get; set; }
    }
}