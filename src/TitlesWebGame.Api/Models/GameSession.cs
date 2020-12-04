using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TitlesWebGame.Api.Models
{
    public class GameSession
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
        
        public void SetPlayingStatus(bool isPlaying)
        {
            IsPlaying = isPlaying;
        }
        
        public Task PlayNewRound(GameRoundInfo gameRoundInfo)
        {
            if (gameRoundInfo is MultipleChoiceRoundInfo roundInfo)
            {
                _currentGameRound =
                    new MultipleChoiceGameRound(roundInfo.Answer, roundInfo.RewardPoints, roundInfo.RoundTimeMs);
            }            
            
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

        public List<GameSessionPlayer> GetPlayers()
        {
            return _players;
        }
    }
}