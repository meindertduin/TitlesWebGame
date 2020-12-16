namespace TitlesWebGame.Domain.Entities
{
    public class CompetitiveArtistRoundInfo : GameRoundInfo
    { 
        public int RoundsAmount { get; set; }
        public int PaintingRoundTimeMs { get; set; }
        public int VotingRoundTimeMs { get; set; }
    }
}