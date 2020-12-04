using TitlesWebGame.Domain.Enums;

namespace TitlesWebGame.Domain.Entities
{
    public class MultipleChoiceRoundInfo : GameRoundInfo
    {
        public int Answer { get; set; }
        public int RoundTimeMs { get; set; }
        public int RewardPoints { get; set; }
        public string[] Choices { get; set; }
        public TitleCategory TitleCategory { get; set; }
    }
}