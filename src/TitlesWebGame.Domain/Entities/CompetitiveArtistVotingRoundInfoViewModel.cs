using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Domain.Entities
{
    public class CompetitiveArtistVotingRoundInfoViewModel : GameRoundInfoViewModel
    {
        public string[] Choices { get; set; }
    }
}