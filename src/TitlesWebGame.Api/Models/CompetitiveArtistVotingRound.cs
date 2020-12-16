using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Models
{
    public class CompetitiveArtistVotingRound : IGameRound
    {
        private readonly int _roundTimeMs;
        private readonly int _rewardPoints;
        private bool _canCommitAnswer;
        private List<CompetitiveArtistVotingRoundAnswer> _answers = new();

        public CompetitiveArtistVotingRound(int roundTimeMs, int rewardPoints)
        {
            _roundTimeMs = roundTimeMs;
            _rewardPoints = rewardPoints;
        }
        
        public bool AddAnswer(GameRoundAnswer answer)
        {
            if (_canCommitAnswer)
            {
                _answers.Add(answer as CompetitiveArtistVotingRoundAnswer);
                return true;   
            }

            return false;
        }

        public Task PlayRound()
        {
            _canCommitAnswer = true;
            return Task.Delay(_roundTimeMs);
        }

        public List<(string, int)> StopRound()
        {
            _canCommitAnswer = false;

            var winner = _answers
                .GroupBy(x => x.Answer)
                .OrderByDescending(x => x.Count())
                .Select(x => (x.Key, x.Count()))
                .ToList();
            
            if (winner[0].Item2 == winner[1].Item2)
            {
                return new List<(string, int)>()
                {
                    (winner[0].Key, _rewardPoints / 2),
                    (winner[1].Key, _rewardPoints / 2),
                };
            }
            
            return new List<(string, int)>()
            {
                (winner[0].Key, _rewardPoints),
                (winner[1].Key, _rewardPoints),
            };
        }

        public List<(string ConnectionId, string Data)> GetRoundAnswersData()
        {
            throw new System.NotImplementedException();
        }
    }
}