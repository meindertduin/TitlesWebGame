using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using TitlesWebGame.Api.Hubs;

namespace TitlesWebGame.Api.Models
{
    public class GameSession
    {
        private readonly IHubContext<TitlesGameHub> _titlesGameHub;
        public string RoomKey { get; set; }
        public string OwnerConnectionId { get; set; }
        public bool IsPlaying { get; private set; }


        private List<GameSessionPlayer> _players = new();
        
        private IGameRound _currentGameRound;
        private GameRoundInfo[][] _gameRoundsInfo;

        public GameSession(IHubContext<TitlesGameHub> titlesGameHub)
        {
            _titlesGameHub = titlesGameHub;
        }

        public async Task PlayGame(string connectionId)
        {
            if (connectionId == OwnerConnectionId)
            {
                IsPlaying = true;
            
                // get info of all rounds being played
                // Todo: make this into a factory class or some sort
            
                _gameRoundsInfo = new[]
                {
                    new[] { new MultipleChoiceRoundInfo()
                    {
                        Answer = 1,
                        Choices = new []{ "bear", "zebra", "giraffe", "crocodile"},
                        RewardPoints = 500,
                        GameRoundsType = GameRoundsType.MultipleChoiceRound,
                        RoundStatement = "What animal is primarily known for having stripes",
                        RoundTimeMs = 100,
                        TitleCategory = TitleCategories.Scientist,
                    }}
                };
            
                // loops through all title rounds
                foreach (var titleRound in _gameRoundsInfo)
                {
                    // loops through all gameRoundInfo in titleRound
                    foreach (var gameRoundInfo in titleRound)
                    {
                        await PlayNewRound(gameRoundInfo);
                    }
                }

                IsPlaying = false;
            }
        }
        
        private async Task PlayNewRound(GameRoundInfo gameRoundInfo)
        {
            if (gameRoundInfo is MultipleChoiceRoundInfo roundInfo)
            {
                _currentGameRound =
                    new MultipleChoiceGameRound(roundInfo.Answer, roundInfo.RewardPoints, roundInfo.RoundTimeMs);
            }            
            
            // await the game round being played
            var scores = await _currentGameRound.PlayRound();
            
            AddScores(scores);
        }

        public bool AddAnswer(GameRoundAnswer gameRoundAnswer)
        {
            if (_players.FirstOrDefault(x => x.ConnectionId == gameRoundAnswer.ConnectionId) != null)
            {
                return _currentGameRound.AddAnswer(gameRoundAnswer);
            }

            return false;
        }

        private void AddScores(List<(string, int)> scores)
        {
            // adds tuples score to players
            foreach (var score in scores)
            {
                var player = _players.Find(p => p.ConnectionId == score.Item1);
                
                if (player != null)
                {
                    player.CurrentPoints += score.Item2;
                }
            }
        }

        public bool AddPlayer(GameSessionPlayer player)
        {
            if (IsPlaying == false)
            {
                _players.Add(player);
                return true;
            }

            return false;
        }
    }
}