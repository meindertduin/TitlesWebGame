namespace TitlesWebGame.Domain.Entities
{
    public class CompetitiveArtistRoundInfo : GameRoundInfo
    {
        public (string ConnectionId, string RoundStatement)[] MatchUps { get; set; }
        public int RoundsAmount { get; set; }
    }
}