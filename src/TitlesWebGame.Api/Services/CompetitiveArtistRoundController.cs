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
            
            // make match ups for everyone
            List<(string PlayerOne, string PlayerTwo, string roundStatement)> matchUps = new();

            if (players.Count % 2 != 0)
            {
                gameSessionState.AddBot();
            }
                
            players.Shuffle();
                
            for (int i = 0; i < players.Count; i+=2)
            {
                // get random round statement
                matchUps.Add((players[i].ConnectionId, players[i + 1].ConnectionId, roundStatement));
            }
                
            // notify players of painting round info and get them painting!
            var paintingRoundInfoVm = new CanvasPaintingRoundInfoViewModel()
            {
                RoundStatement = roundStatement,
                RoundTimeMs = roundInfo.PaintingRoundTimeMs,
                GameRoundsType = GameRoundsType.CanvasPaintingRound,
                TitleCategory = roundInfo.TitleCategory,
            };

            // Todo: change this into individual sending of viewModels depending on roundStatement and matchUps
            await _clientMessageService.UpdatePlayersOfNewRoundInfo(gameSessionState.RoomKey, paintingRoundInfoVm);
                
            // play the painting round first
            await gameSessionState.PlayNewRound(new CanvasPaintingRound(roundInfo.PaintingRoundTimeMs));

            // get the painting answers
            var answerData = gameSessionState.GetRoundAnswersData();

            // play voting rounds
            for (int i = 0; i < matchUps.Count; i++)
            {
                var answerDataPlayerOne = answerData.FirstOrDefault(x => x.ConnectionId == matchUps[i].PlayerOne);
                var answerDataPlayerTwo = answerData.FirstOrDefault(x => x.ConnectionId == matchUps[i].PlayerTwo);
                    
                // give players voting round info
                var votingRoundInfoVm = new CompetitiveArtistVotingRoundInfoViewModel()
                {
                    RoundTimeMs = roundInfo.VotingRoundTimeMs,
                    RoundStatement = roundStatement,
                    GameRoundsType = GameRoundsType.CompetitiveArtistVotingRound,
                    Choices = new[] {answerDataPlayerOne, answerDataPlayerTwo},
                };

                await _clientMessageService.UpdatePlayersOfNewRoundInfo(gameSessionState.RoomKey, votingRoundInfoVm);
                    
                // play new voting round
                await gameSessionState.PlayNewRound(new CompetitiveArtistVotingRound(roundInfo.VotingRoundTimeMs, roundInfo.RewardPoints));
                var scores = gameSessionState.GetRoundScores();
                gameSessionState.AddScores(scores);
                    
                // voting round review
                await _clientMessageService.UpdatePlayersOfPreviousRoundInfo(gameSessionState.RoomKey, roundInfo);
                await _clientMessageService.UpdatePlayersOfRoundReview(gameSessionState.RoomKey, gameSessionState.GetPlayers());
            }
        }
    }

    public interface IGameRoundController
    {
        Task PlayGameRound(GameSessionState gameSessionState, GameRoundInfo gameRoundInfo);
    }
}