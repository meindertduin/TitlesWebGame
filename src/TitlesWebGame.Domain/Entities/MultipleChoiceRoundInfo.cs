using TitlesWebGame.Domain.Enums;

namespace TitlesWebGame.Domain.Entities
{
    public class MultipleChoiceRoundInfo : GameRoundInfo
    {
        public string RoundStatement { get; set; }
        public string Answer { get; set; }
        public string[] Choices { get; set; }
        public int RoundTimeMs { get; set; }
    }
}