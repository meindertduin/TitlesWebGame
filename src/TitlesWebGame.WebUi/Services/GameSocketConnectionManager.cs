using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services
{
    public class GameSocketConnectionManager
    {
        private readonly GameSocketServerMessageHandler _gameSocketServerMessageHandler;
        private readonly GameSessionSateManager _gameSessionSateManager;
        public HubConnection HubConnection { get; private set; }

        public GameSocketConnectionManager(GameSocketServerMessageHandler gameSocketServerMessageHandler, 
            GameSessionSateManager gameSessionSateManager)
        {
            _gameSocketServerMessageHandler = gameSocketServerMessageHandler;
            _gameSessionSateManager = gameSessionSateManager;
        }
        public async Task ConnectSocket()
        {
            HubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/game")
                .Build();

            SetHubConnectionEventHandlers();

            await HubConnection.StartAsync();
        }

        private void SetHubConnectionEventHandlers()
        {
            RegisterOnServerMessageUpdateEventHandler();

            RegisterOnNewGameRoundInfoEventHandler();

            RegisterOnUpdateSessionStateEventHandler();

            RegisterOnGameEndEventHandler();
        }

        private void RegisterOnGameEndEventHandler()
        {
            HubConnection.On<TitlesGameEndGameResults>("EndGameResultsUpdate", (endGameResults) =>
            {
                // Todo: handle display for end of game
            });
        }

        private void RegisterOnUpdateSessionStateEventHandler()
        {
            HubConnection.On<SessionStateUpdateViewModel>("GameSessionStateUpdate", (sessionState) =>
            {
                // Todo: handle right answers of previous game

                _gameSessionSateManager.SetPlayersState(sessionState.GameSessionPlayers);
            });
        }

        private void RegisterOnNewGameRoundInfoEventHandler()
        {
            HubConnection.On<GameRoundInfoViewModel>("NextRoundInfoUpdate", (GameRoundInfo) =>
            {
                // Todo: display info of next round
            });
        }

        private void RegisterOnServerMessageUpdateEventHandler()
        {
            HubConnection.On<TitlesGameHubMessageModel>("ServerMessageUpdate", (message) =>
                _gameSocketServerMessageHandler.Handle(message));
        }

        public async Task CreateGameSession(string displayName)
        {
            await HubConnection.SendAsync("CreateRoom", displayName);
        }

        public async Task StartGameSession(string roomKey)
        {
            await HubConnection.SendAsync("StartGameSession", roomKey);
        }
    }
}