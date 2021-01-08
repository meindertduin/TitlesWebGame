using TitlesWebGame.Domain.Enums;

namespace TitlesWebGame.Domain.Entities
{
    public class GameRoundInfo
    {
        public GameRoundsType GameRoundsType { get; set; }
        public TitleCategory TitleCategory { get; set; }
        public int RewardPoints { get; set; }
        public int RoundTimeMs { get; set; }
        
    }
}