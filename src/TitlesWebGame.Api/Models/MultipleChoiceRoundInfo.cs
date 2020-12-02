namespace TitlesWebGame.Api.Models
{
    public class MultipleChoiceRoundInfo : GameRoundInfo
    {
        public int Answer { get; set; }
        public int RoundTimeMs { get; set; }
        public int RewardPoints { get; set; }
        public string[] Choices { get; set; }
    }
}