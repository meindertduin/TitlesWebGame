using System.Collections.Generic;
using System.Threading.Tasks;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Models
{
    public class CompetitiveArtistVotingRound : IGameRound
    {
        private readonly int _roundTimeMs;
        private bool _canCommitAnswer;
        private List<CompetitiveArtistVotingRoundAnswer> _answers = new();

        public CompetitiveArtistVotingRound(int roundTimeMs)
        {
            _roundTimeMs = roundTimeMs;
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
            return new List<(string, int)>();
        }

        public List<string> GetRoundAnswersData()
        {
            throw new System.NotImplementedException();
        }
    }
}