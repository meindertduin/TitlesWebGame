using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Models
{
    public class MultipleChoiceGameRound : IGameRound
    {
        private readonly string _answer;
        private readonly int _rewardPoints;
        private readonly int _roundTimeMs;

        private const int BufferTimeMs = 1000;

        private bool _canCommitAnswer;
        
        private List<GameRoundAnswer> _answers = new();
        private DateTime _startTime;

        public MultipleChoiceGameRound(string answer, int rewardPoints, int roundTimeMs)
        {
            _answer = answer;
            _rewardPoints = rewardPoints;
            _roundTimeMs = roundTimeMs;
        }
        
        public bool AddAnswer(GameRoundAnswer answer)
        {
            if (_canCommitAnswer)
            {
                var elapsedTime = (DateTime.Now - _startTime).TotalMilliseconds;

                answer.TimeMs = (int) elapsedTime;
                _answers.Add(answer);
                
                return true;
            }

            return false;
        }

        public Task PlayRound()
        {
            _startTime = DateTime.Now;
            _canCommitAnswer = true;

            return Task.Delay(_roundTimeMs);
        }

        public List<(string, int)>StopRound()
        {
            _canCommitAnswer = false;
            var scores = CalculatePlayersPoints();

            return scores;
        }

        public List<GameRoundAnswer> GetRoundAnswers()
        {
            return _answers;
        }

        private List<(string, int)> CalculatePlayersPoints()
        {
            List<(string, int)> scores = new List<(string, int)>();

            // iterate throug each answer and get the score
            foreach (var answer in _answers)
            {
                int score = 0;
                if (answer.Answer == _answer)
                {
                    // if answer time is above buffer time, points will be decreased linearly depending on the amount of time
                    float multiplier = 1;
                    
                    if (answer.TimeMs > BufferTimeMs)
                    {
                        float overBufferTimeMs =  answer.TimeMs - BufferTimeMs;
                        float nonOverlappingBufferTimeMs = _roundTimeMs - BufferTimeMs;
                        multiplier -= (overBufferTimeMs / nonOverlappingBufferTimeMs);
                    }
                    
                    score += (int) (_rewardPoints * multiplier);
                }

                scores.Add((answer.ConnectionId, score));
            }

            return scores;
        }
        
        public List<GameRoundAnswer> GetRoundAnswers(string[] connections)
        {
            return _answers.Where(x => connections.Contains(x.ConnectionId)).ToList();
        }
    }
}