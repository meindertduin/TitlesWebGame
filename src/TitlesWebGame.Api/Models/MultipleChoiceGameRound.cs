using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TitlesWebGame.Api.Models
{
    public class MultipleChoiceGameRound : IGameRound
    {
        private readonly int _answer;
        private readonly int _rewardPoints;
        private readonly int _roundTimeMs;

        private const int BufferTimeMs = 1;

        private bool _canCommitAnswer;
        
        private List<MultipleChoiceAnswer> _playerAnswers = new();
        private DateTime _startTime;

        public MultipleChoiceGameRound(int answer, int rewardPoints, int roundTimeMs)
        {
            _answer = answer;
            _rewardPoints = rewardPoints;
            _roundTimeMs = roundTimeMs;
        }
        
        public void AddAnswer(string connectionId, int answer)
        {
            if (_canCommitAnswer)
            {
                var elapsedTime = (DateTime.Now - _startTime).TotalMilliseconds;
            
                _playerAnswers.Add(new MultipleChoiceAnswer()
                {
                    ConnectionId = connectionId,
                    Answer = answer,
                    TimeMs = (int) elapsedTime,
                });
            }
        }

        public async Task<List<(string, int)>> PlayRound()
        {
            _startTime = DateTime.Now;
            _canCommitAnswer = true;

            await Task.Delay(_roundTimeMs);
            return StopRound();
        }

        private List<(string, int)>StopRound()
        {
            _canCommitAnswer = false;
            var scores = CalculatePlayersPoints();

            return scores;
        }

        private List<(string, int)> CalculatePlayersPoints()
        {
            List<(string, int)> scores = new List<(string, int)>();

            // iterate throug each answer and get the score
            foreach (var answer in _playerAnswers)
            {
                int score = 0;
                if (answer.Answer == _answer)
                {
                    // if answer time is above buffer time, points will be decreased linearly depending on the amount of time
                    score += _rewardPoints * answer.TimeMs > BufferTimeMs ? ((_roundTimeMs - BufferTimeMs) / answer.TimeMs) : 1;
                }

                scores.Add((answer.ConnectionId, score));
            }

            return scores;
        }
    }
}