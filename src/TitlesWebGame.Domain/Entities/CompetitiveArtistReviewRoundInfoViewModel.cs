using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Domain.Entities
{
    public class CompetitiveArtistReviewRoundInfoViewModel : GameRoundInfoViewModel
    {
        public GameSessionPlayer Winner { get; set; }
    }
}