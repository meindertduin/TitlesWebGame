using System.Collections.Generic;

namespace TitlesWebGame.Api.Models
{
    public abstract class GameRoundInfo
    {
        public string RoundStatement { get; set; }
        public GameRoundsType GameRoundsType { get; set; }
    }
}