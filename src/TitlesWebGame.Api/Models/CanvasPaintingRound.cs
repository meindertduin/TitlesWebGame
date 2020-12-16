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
        private List<CanvasPaintingAnswer> _answers = new();

        public CanvasPaintingRound(int roundTimeMs)
        {
            _roundTimeMs = roundTimeMs;
        }
        public bool AddAnswer(GameRoundAnswer answer)
        {
            if (_canCommitAnswer)
            {
                _answers.Add(answer as CanvasPaintingAnswer);
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
            return _answers.Select(x => x.Answer).ToList();
        }
    }
}