namespace TitlesWebGame.Domain.Entities
{
    public class GameSessionPlayer
    {
        public string DisplayName { get; set; }
        public string ConnectionId { get; set; }
        public int CurrentPoints { get; set; }
    }
}