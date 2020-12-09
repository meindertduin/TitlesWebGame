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
        
        public event Action OnSessionStateChanged;
        public event Action OnSessionInit;
        public void InitializeNewState(GameSessionInitViewModel gameSessionInitModel)
        {
            RoomKey = gameSessionInitModel.RoomKey;
            OwnerConnectionId = gameSessionInitModel.OwnerConnectionId;
            Players = gameSessionInitModel.GameSessionPlayers;
            GameSessionPlayer = gameSessionInitModel.CurrentPlayer;
            IsPlaying = false;
            HasEnded = false;
            PreviousRoundInfo = null;
            NextRoundInfo = null;
            
            NotifyStateInit();
            NotifyStateChanged();
        }

        public bool IsOwner() => GameSessionPlayer.ConnectionId == OwnerConnectionId;

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

        public void SetSessionGameStatUpdateInfo(SessionStateUpdateViewModel sessionStateUpdate)
        {
            Players = sessionStateUpdate.GameSessionPlayers;
            PreviousRoundInfo = sessionStateUpdate.PreviousRoundInfo;
            GameSessionPlayer = Players.FirstOrDefault(x => x.ConnectionId == GameSessionPlayer.ConnectionId);

            NotifyStateChanged();
        }

        public void SetNextRoundInfo(GameRoundInfoViewModel gameRoundInfoViewModel)
        {
            NextRoundInfo = gameRoundInfoViewModel;
            
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnSessionStateChanged?.Invoke();
        private void NotifyStateInit() => OnSessionInit?.Invoke();
        
    }
}