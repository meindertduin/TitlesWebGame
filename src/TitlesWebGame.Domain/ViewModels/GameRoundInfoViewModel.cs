using TitlesWebGame.Domain.Enums;

namespace TitlesWebGame.Domain.ViewModels
{
    public class GameRoundInfoViewModel
    {
        public string RoundStatement { get; set; }
        public GameRoundsType GameRoundsType { get; set; }
        public TitleCategory TitleCategory { get; set; }
    }
}