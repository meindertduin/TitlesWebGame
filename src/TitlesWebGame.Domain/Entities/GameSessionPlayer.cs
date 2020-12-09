using System.Collections.Generic;
using TitlesWebGame.Domain.Enums;

namespace TitlesWebGame.Domain.Entities
{
    public class GameSessionPlayer
    {
        public string DisplayName { get; set; }
        public string ConnectionId { get; set; }
        public int CurrentPoints { get; set; }
        public int RoundAwardedPoints { get; set; }
        public List<TitleCategory> WonTitles { get; private set; } = new ();

        public void AddTitleCategory(TitleCategory titleCategory)
        {
            WonTitles.Add(titleCategory);
        }
    }
}