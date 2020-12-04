using TitlesWebGame.Domain.Enums;

namespace TitlesWebGame.Domain.Entities
{
    public class GameRoundInfo
    {
        public string RoundStatement { get; set; }
        public GameRoundsType GameRoundsType { get; set; }
        public TitleCategory TitleCategory { get; set; }
    }
}