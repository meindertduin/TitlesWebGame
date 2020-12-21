using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Models
{
    public class CanvasPaintingRound : IGameRound
    {
        private readonly int _roundTimeMs;
        private bool _canCommitAnswer;
        private List<GameRoundAnswer> _answers = new();

        public CanvasPaintingRound(int roundTimeMs)
        {
            _roundTimeMs = roundTimeMs;
        }
        public bool AddAnswer(GameRoundAnswer answer)
        {
            if (_canCommitAnswer)
            {
                _answers.Add(answer);
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

        public List<GameRoundAnswer> GetRoundAnswers()
        {
            return _answers;
        }

        public List<GameRoundAnswer> GetRoundAnswers(string[] connections)
        {
            return _answers.Where(x => connections.Contains(x.ConnectionId)).ToList();
        }
    }
}