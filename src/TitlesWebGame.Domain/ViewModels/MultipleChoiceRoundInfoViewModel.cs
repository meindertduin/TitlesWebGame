namespace TitlesWebGame.Domain.ViewModels
{
    public class MultipleChoiceRoundInfoViewModel : GameRoundInfoViewModel
    {
        public int RoundTimeMs { get; set; }
        public int RewardPoints { get; set; }
        public string[] Choices { get; set; }
    }
}