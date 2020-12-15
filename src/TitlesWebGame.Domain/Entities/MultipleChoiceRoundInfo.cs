using TitlesWebGame.Domain.Enums;

namespace TitlesWebGame.Domain.Entities
{
    public class MultipleChoiceRoundInfo : GameRoundInfo
    {
        public string Answer { get; set; }
        public string[] Choices { get; set; }
        
        public string RoundStatement { get; set; }
    }
}