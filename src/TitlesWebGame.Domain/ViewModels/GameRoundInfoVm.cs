using TitlesWebGame.Domain.Enums;

namespace TitlesWebGame.Domain.ViewModels
{
    public class GameRoundInfoVm
    {
        public int Id { get; set; }
        public GameRoundsType GameRoundsType { get; set; }
        public TitleCategory TitleCategory { get; set; }
        public int RewardPoints { get; set; }
        public int RoundTimeMs { get; set; }
        public int PaintingRoundTimeMs { get; set; }
        public int VotingRoundTimeMs { get; set; }
        public string RoundStatement { get; set; }
        public string Answer { get; set; }
        public string Choices { get; set; }
    }
}