using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Domain.Entities
{
    public class CompetitiveArtistReviewRoundInfoViewModel : GameRoundInfoViewModel
    {
        public GameSessionPlayer Winner { get; set; }
        public bool IsDraw { get; set; }
        public string[] Choices { get; set; }
    }
}