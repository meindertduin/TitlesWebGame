using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitlesWebGame.Api.Extensions;
using TitlesWebGame.Api.Models;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.Enums;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Services
{
    public class CompetitiveArtistRoundController : IGameRoundController
    {
        private readonly IGameSessionClientMessageService _clientMessageService;
        private const int LatencyBufferTimeMs = 1500;

        public CompetitiveArtistRoundController(IGameSessionClientMessageService clientMessageService)
        {
            _clientMessageService = clientMessageService;
        }

        public async Task PlayGameRound(GameSessionState gameSessionState, GameRoundInfo gameRoundInfo)
        {
            var roundInfo = gameRoundInfo as CompetitiveArtistRoundInfo;
            if (roundInfo == null)
            {
                throw new InvalidCastException(nameof(roundInfo));
            }

            var players = gameSessionState.GetPlayers();
            var roundStatement = "Draw a car with only triangles";

            var matchUps = GenerateMatchUps(gameSessionState, players, roundStatement);

            await InitializePaintingRound(gameSessionState, roundStatement, roundInfo);

            // play the painting round first
            await gameSessionState.PlayNewRound(new CanvasPaintingRound(roundInfo.PaintingRoundTimeMs));
            
            await PlayVotingRounds(gameSessionState, matchUps, roundInfo, roundStatement);

            // voting round review
            await _clientMessageService.UpdatePlayersOfPreviousRoundInfo(gameSessionState.RoomKey, roundInfo);
            await _clientMessageService.UpdatePlayersOfRoundReview(gameSessionState.RoomKey,
                gameSessionState.GetPlayers());
        }

        private List<(string PlayerOne, string PlayerTwo, string roundStatement)> GenerateMatchUps(
            GameSessionState gameSessionState,
            List<GameSessionPlayer> players, string roundStatement)
        {
            // make match ups for everyone
            List<(string PlayerOne, string PlayerTwo, string roundStatement)> matchUps = new();

            if (players.Count % 2 != 0)
            {
                gameSessionState.AddBot();
            }

            players.Shuffle();

            for (int i = 0; i < players.Count; i += 2)
            {
                // get random round statement
                matchUps.Add((players[i].ConnectionId, players[i + 1].ConnectionId, roundStatement));
            }

            return matchUps;
        }

        private Task InitializePaintingRound(GameSessionState gameSessionState, string roundStatement,
            CompetitiveArtistRoundInfo roundInfo)
        {
            // notify players of painting round info and get them painting!
            var paintingRoundInfoVm = new CanvasPaintingRoundInfoViewModel()
            {
                RoundStatement = roundStatement,
                RoundTimeMs = roundInfo.PaintingRoundTimeMs,
                GameRoundsType = GameRoundsType.CanvasPaintingRound,
                TitleCategory = roundInfo.TitleCategory,
            };

            // Todo: change this into individual sending of viewModels depending on roundStatement and matchUps
            return _clientMessageService.UpdatePlayersOfNewRoundInfo(gameSessionState.RoomKey, paintingRoundInfoVm);
        }

        private async Task PlayVotingRounds(GameSessionState gameSessionState,
            List<(string PlayerOne, string PlayerTwo, string roundStatement)> matchUps,
            CompetitiveArtistRoundInfo roundInfo, string roundStatement)
        {
            // play voting rounds
            for (int i = 0; i < matchUps.Count; i++)
            {
                // give players voting round info
                var votingRoundInfoVm = new CompetitiveArtistVotingRoundInfoViewModel()
                {
                    RoundTimeMs = roundInfo.VotingRoundTimeMs,
                    RoundStatement = roundStatement,
                    GameRoundsType = GameRoundsType.CompetitiveArtistVotingRound,
                    Choices = new [] {matchUps[i].PlayerOne, matchUps[i].PlayerTwo}
                };

                await _clientMessageService.UpdatePlayersOfNewRoundInfo(gameSessionState.RoomKey, votingRoundInfoVm);

                if (i == 0)
                {
                    await Task.Delay(3000);
                }

                List<GameRoundAnswer> defaultScores = new()
                {
                    new()
                    {
                        Answer = votingRoundInfoVm.Choices[0],
                    },
                    new()
                    {
                        Answer = votingRoundInfoVm.Choices[1],
                    },
                };
                
                // play new voting round
                await gameSessionState.PlayNewRound(new CompetitiveArtistVotingRound(roundInfo.VotingRoundTimeMs,
                    roundInfo.RewardPoints, defaultScores));
                
                var scores = gameSessionState.GetRoundScores();
                gameSessionState.AddScores(scores);

                // notify players of winner
                var highestScore = scores.OrderByDescending(x => x.Item2).ToArray()[0];
                
                var winner = gameSessionState
                    .GetPlayers()
                    .FirstOrDefault(x => x.ConnectionId == highestScore.Item1);

                var reviewInfoVm = new CompetitiveArtistReviewRoundInfoViewModel()
                {
                    RoundTimeMs = 2000,
                    RoundStatement = roundInfo.RoundStatement,
                    GameRoundsType = GameRoundsType.CompetitiveArtistReviewRound,
                    Winner = winner,
                };

                await _clientMessageService.UpdatePlayersOfNewRoundInfo(gameSessionState.RoomKey, reviewInfoVm);
                await Task.Delay(3000);
            }

            gameSessionState.TryRemoveBot();
        }
    }
}
