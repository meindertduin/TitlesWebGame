using TitlesWebGame.Domain.Enums;

namespace TitlesWebGame.Api.Models
{
    public class MultipleChoiceConversionModel
    {
        public string RoundStatement { get; set; }
        public string Answer { get; set; }
        public string Choices { get; set; }
        public GameRoundsType GameRoundsType { get; set; }
        public TitleCategory TitleCategory { get; set; }
        public int RewardPoints { get; set; }
        public int RoundTimeMs { get; set; }
    }
}