using System;
using System.Collections.Generic;
using System.Linq;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;
using TitlesWebGame.WebUi.Models;

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
        
        public event Action OnSessionStateChanged;
        
        public void InitializeNewState(GameSessionInitViewModel gameSessionInitModel)
        {
            RoomKey = gameSessionInitModel.RoomKey;
            OwnerConnectionId = gameSessionInitModel.OwnerConnectionId;
            Players = gameSessionInitModel.GameSessionPlayers;
            NotifyStateChanged();
        }
        
        public void SetPlayerStates(List<GameSessionPlayer> playerStates)
        {
            Players = playerStates;
            NotifyStateChanged();
        }

        public void AddPlayer(GameSessionPlayer player)
        {
            Players.Add(player);
            NotifyStateChanged();
        }

        public void RemovePlayer(string connectionId)
        {
            var leavingPlayer = Players.FirstOrDefault(x => x.ConnectionId == connectionId);
            Players.Remove(leavingPlayer);
            NotifyStateChanged();
        }

        public void SetPlayingStatus(bool isPlaying)
        {
            IsPlaying = isPlaying;
            NotifyStateChanged();
        }

        public void SetGameEndedStatus(bool hasEnded)
        {
            HasEnded = hasEnded;
            NotifyStateChanged();
        }

        public void SetPreviousRoundInfo(GameRoundInfo gameRoundInfo)
        {
            PreviousRoundInfo = gameRoundInfo;
            NotifyStateChanged();
        }

        public void SetNextRoundInfo(GameRoundInfoViewModel gameRoundInfoViewModel)
        {
            NextRoundInfo = gameRoundInfoViewModel;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnSessionStateChanged?.Invoke();
    }
}