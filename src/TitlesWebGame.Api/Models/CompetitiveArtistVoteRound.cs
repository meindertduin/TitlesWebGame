using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Models
{
    public class CompetitiveArtistVoteRound : IGameRound
    {
        private readonly CompetitiveArtistRoundInfo _roundInfo;
        private readonly int _roundTimeMs;
        private readonly int _rewardPoints;
        private bool _canCommitAnswer;

        private List<CompetitiveArtistRoundAnswer> _roundAnswers = new();

        public CompetitiveArtistVoteRound(CompetitiveArtistRoundInfo roundInfo, int roundTimeMs, int rewardPoints)
        {
            _roundInfo = roundInfo;
            _roundTimeMs = roundTimeMs;
            _rewardPoints = rewardPoints;
        }
        public bool AddAnswer(GameRoundAnswer answer)
        {
            if (answer is CompetitiveArtistRoundAnswer roundAnswer && _canCommitAnswer)
            {
                _roundAnswers.Add(roundAnswer);
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
            var winnerPlayerId = _roundAnswers
                .Select(answer => answer.ConnectionId)
                .GroupBy(i => i)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .First();


            return new List<(string, int)>()
            {
                (winnerPlayerId, _rewardPoints),
            };
        }
    }
}