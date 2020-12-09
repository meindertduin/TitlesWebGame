using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using TitlesWebGame.Api.Models;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services
{
    public class GameSocketConnectionManager
    {
        private readonly GameSocketServerMessageHandler _gameSocketServerMessageHandler;
        public HubConnection HubConnection { get; private set; }

        public GameSocketConnectionManager(GameSocketServerMessageHandler gameSocketServerMessageHandler)
        {
            _gameSocketServerMessageHandler = gameSocketServerMessageHandler;
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
        }
        
        private void RegisterOnServerMessageUpdateEventHandler()
        {
            HubConnection.On<TitlesGameHubMessageModel>("ServerMessageUpdate", (message) =>
                _gameSocketServerMessageHandler.Handle(message));
        }

        public async Task JoinGameSession(string roomKey, string displayName)
        {
            await HubConnection.SendAsync("ConnectToRoom", roomKey, displayName);
        }
        
        public async Task CreateGameSession(string displayName)
        {
            await HubConnection.SendAsync("CreateRoom", displayName);
        }

        public async Task StartGameSession(string roomKey)
        {
            await HubConnection.SendAsync("StartGameSession", roomKey);
        }

        public async Task SendAnswer(string roomKey, GameRoundAnswer gameRoundAnswer)
        {
            await HubConnection.SendAsync("AnswerChoice", roomKey, gameRoundAnswer);
        }
    }
}