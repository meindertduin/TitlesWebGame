namespace TitlesWebGame.Domain.Entities
{
    public class CompetitiveArtistRoundInfo : GameRoundInfo
    { 
        public int PaintingRoundTimeMs { get; set; }
        public int VotingRoundTimeMs { get; set; }
        public string RoundStatement { get; set; }
    }
}