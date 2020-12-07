using System;
using System.Collections.Generic;
using System.Linq;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services
{
    public class GameSessionState
    {
        public GameSessionPlayer GameSessionPlayer { get; private set; }
        public string RoomKey { get; private set; }
        public bool HasEnded { get; private set; }
        public List<GameSessionPlayer> Players { get; private set; }
        public string OwnerConnectionId { get; private set; }
        public bool IsPlaying { get; private set; }
        public GameRoundInfo PreviousRoundInfo { get; private set; }
        public GameRoundInfoViewModel NextRoundInfo { get; private set; }
        
        public event Action OnChange;
        public void SetPlayerStates(List<GameSessionPlayer> playerStates)
        {
            Players = playerStates;
        }

        public void AddPlayer(GameSessionPlayer player)
        {
            Players.Add(player);
        }

        public void RemovePlayer(string connectionId)
        {
            var leavingPlayer = Players.FirstOrDefault(x => x.ConnectionId == connectionId);
            Players.Remove(leavingPlayer);
        }

        public void SetPlayingStatus(bool isPlaying)
        {
            IsPlaying = isPlaying;
        }

        public void SetGameEndedStatus(bool hasEnded)
        {
            HasEnded = hasEnded;
        }

        public void SetPreviousRoundInfo(GameRoundInfo gameRoundInfo)
        {
            PreviousRoundInfo = gameRoundInfo;
        }

        public void SetNextRoundInfo(GameRoundInfoViewModel gameRoundInfoViewModel)
        {
            NextRoundInfo = gameRoundInfoViewModel;
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}