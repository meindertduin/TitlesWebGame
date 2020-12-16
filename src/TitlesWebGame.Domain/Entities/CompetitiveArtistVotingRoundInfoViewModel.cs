using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Domain.Entities
{
    public class CompetitiveArtistVotingRoundInfoViewModel : GameRoundInfoViewModel
    {
        public (string ConnectionId, string Data)[] Choices { get; set; }
    }
}