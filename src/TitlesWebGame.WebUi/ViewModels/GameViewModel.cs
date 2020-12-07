using System;
using System.Collections.Generic;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;
using TitlesWebGame.WebUi.Services;

namespace TitlesWebGame.WebUi.ViewModels
{
    public class GameViewModel : BaseViewModel, IDisposable
    {
        private readonly GameSessionState _gameSessionState;

        public GameViewModel(GameSessionState gameSessionState)
        {
            _gameSessionState = gameSessionState;
            _gameSessionState.OnSessionStateChanged += OnGameStateChanged;

            // Initialize values from gameSessionState
            IsOwner = _gameSessionState.IsOwner();
            
        }
        private void OnGameStateChanged()
        {
            IsOwner = _gameSessionState.IsOwner();
        }
        
        private bool _isOwner;
        public bool IsOwner
        {
            get => _isOwner;
            set => SetValue(ref _isOwner, value);
        }

        private GameSessionPlayer _player;
        public GameSessionPlayer Player
        {
            get => _player;
            set => SetValue(ref _player, value);
        }

        private List<GameSessionPlayer> _sessionPlayers;
        public List<GameSessionPlayer> SessionPlayers
        {
            get => _sessionPlayers;
            set => SetValue(ref _sessionPlayers, value);
        }

        private bool _hasEnded;
        public bool HasEnded
        {
            get => _hasEnded;
            set => SetValue(ref _hasEnded, value);
        }

        private GameRoundInfoViewModel _nextRoundInfo;
        public GameRoundInfoViewModel NextRoundInfo
        {
            get => _nextRoundInfo;
            set => SetValue(ref _nextRoundInfo, value);
        }

        private GameRoundInfo _previousRoundInfo;
        public GameRoundInfo PreviousRoundInfo
        {
            get => _previousRoundInfo;
            set => SetValue(ref _previousRoundInfo, value);
        }

        public void Dispose()
        {
            _gameSessionState.OnSessionStateChanged -= OnGameStateChanged;
        }
    }
    
}