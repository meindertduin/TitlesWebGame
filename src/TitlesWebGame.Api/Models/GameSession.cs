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
            // simulated round info but in future will be obtained from database
            var roundInfo = new MultipleChoiceRoundInfo()
            {
                Answer = 1,
                Choices = new []{ "bear", "zebra", "giraffe", "crocodile"},
                RewardPoints = 500,
                GameRoundsType = GameRoundsType.MultipleChoiceRound,
                RoundStatement = "What animal is primarily known for having stripes"
            };

            // initialize the new round
            _currentGameRound =
                new MultipleChoiceGameRound(roundInfo.Answer, roundInfo.RewardPoints, roundInfo.RoundTimeMs);
            
            // await the game round being played
            var scores = await _currentGameRound.PlayRound();
            
            AddScores(scores);
        }

        public void AddAnswer(GameRoundAnswer gameRoundAnswer)
        {
            _currentGameRound.AddAnswer(gameRoundAnswer);
        }

        private void AddScores(List<(string, int)> scores)
        {
            // adds tuples score to players
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