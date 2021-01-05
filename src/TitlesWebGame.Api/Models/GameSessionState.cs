using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.Enums;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Models
{
    public class GameSessionState
    {
        public string RoomKey { get; set; }
        public string OwnerConnectionId { get; set; }
        public bool IsPlaying { get; private set; }
        
        private List<GameSessionPlayer> _players = new();
        
        private IGameRound _currentGameRound;
        
        private IEnumerator<GameRoundInfo> _gameRoundInfoIterator;

        public void SetRoundInfo(List<GameRoundInfo> gameRoundInfos)
        {
            _gameRoundInfoIterator = gameRoundInfos.GetEnumerator();
        }

        public GameRoundInfo GetNextRound()
        {
            _gameRoundInfoIterator.MoveNext();
            return _gameRoundInfoIterator.Current;
        }

        public void EndTitlesRound(TitleCategory contestingCategory)
        {
            // calculate player most points
            var titlesRoundWinner = _players.OrderByDescending(player => player.CurrentPoints).First();
            // add title to player
            titlesRoundWinner.AddTitleCategory(contestingCategory);
        }

        public void ResetPlayerPoints()
        {
            foreach (var player in _players)
            {
                player.CurrentPoints = 0;
            }
        }
        
        public void SetPlayingStatus(bool isPlaying)
        {
            IsPlaying = isPlaying;
        }
        
        public Task PlayNewRound(IGameRound gameRound)
        {
            _currentGameRound = gameRound;
            // await the game round being played
            return _currentGameRound.PlayRound();
        }

        public List<(string, int)> GetRoundScores()
        {
            return _currentGameRound.StopRound();
        }

        public void AddScores(List<(string, int)> scores)
        {
            // adds tuples score to players
            foreach (var score in scores)
            {
                var player = _players.Find(p => p.ConnectionId == score.Item1);
                
                if (player != null)
                {
                    player.CurrentPoints += score.Item2;
                    player.RoundAwardedPoints = score.Item2;
                }
            }
        }
        public bool AddAnswer(GameRoundAnswer gameRoundAnswer)
        {
            if (_players.FirstOrDefault(x => x.ConnectionId == gameRoundAnswer.ConnectionId) != null)
            {
                return _currentGameRound.AddAnswer(gameRoundAnswer);
            }

            return false;
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

        public bool RemovePlayer(string connectionId, out string newOwnerConId)
        {
            var player = _players.FirstOrDefault(g => g.ConnectionId == connectionId);
            if (player != null)
            {
                // assign someone else owner if player is owner if there exists another player
                if (OwnerConnectionId == player.ConnectionId)
                {
                    var newOwner = _players
                        .FirstOrDefault(x => x.ConnectionId != connectionId && x.ConnectionId != "bot");
                    if (newOwner != null)
                    {
                        newOwnerConId = newOwner.ConnectionId;
                        OwnerConnectionId = newOwner.ConnectionId;
                        _players.Remove(player);
                        return true;
                    }
                }
                
                _players.Remove(player);
                newOwnerConId = String.Empty;
                return true;
            }

            newOwnerConId = String.Empty;
            return false;
        }

        public void AddBot()
        {
            _players.Add(new GameSessionPlayer()
            {
                ConnectionId = "bot",
                DisplayName = "Mr Bot",
            });
        }

        public bool TryRemoveBot()
        {
            var bot = _players.FirstOrDefault(x => x.ConnectionId == "bot");
            if (bot != null)
            {
                _players.Remove(bot);
                return true;
            }

            return false;
        }

        public List<GameSessionPlayer> GetPlayers()
        {
            return _players;
        }

        public int GetPlayersCount()
        {
            return _players.Count;
        }
        
        public List<GameRoundAnswer> GetRoundAnswers()
        {
            return _currentGameRound.GetRoundAnswers();
        }

        public List<GameRoundAnswer> GetRoundAnswers(string[] connections)
        {
            return _currentGameRound.GetRoundAnswers(connections);
        }
    }
}