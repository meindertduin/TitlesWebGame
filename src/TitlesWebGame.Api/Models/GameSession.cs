using System.Collections.Generic;
using System.Threading.Tasks;

namespace TitlesWebGame.Api.Models
{
    public class GameSession
    {
        public string RoomKey { get; set; }
        public List<GameSessionPlayer> Players { get; set; }

        private IGameRound _currentGameRound;

        public async Task PlayNewRound()
        {
            // get new round info
            var roundInfo = new MultipleChoiceRoundInfo();

            _currentGameRound =
                new MultipleChoiceGameRound(roundInfo.Answer, roundInfo.RewardPoints, roundInfo.RoundTimeMs);
            var scores = await _currentGameRound.PlayRound();
            AddScores(scores);
        }

        private void AddScores(List<(string, int)> scores)
        {
            foreach (var score in scores)
            {
                var player = Players.Find(p => p.ConnectionId == score.Item1);
                
                if (player != null)
                {
                    player.CurrentPoints += score.Item2;
                }
            }
        }
    }
}