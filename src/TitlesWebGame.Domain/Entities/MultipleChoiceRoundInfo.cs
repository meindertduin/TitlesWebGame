using TitlesWebGame.Domain.Enums;

namespace TitlesWebGame.Domain.Entities
{
    public class MultipleChoiceRoundInfo : GameRoundInfo
    {
        public string[] Choices { get; set; }
    }
}