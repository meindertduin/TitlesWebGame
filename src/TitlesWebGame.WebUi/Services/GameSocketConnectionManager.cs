using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services
{
    public class GameSocketConnectionManager
    {
        private readonly GameSocketServerMessageHandler _gameSocketServerMessageHandler;
        private readonly HttpClient _httpClient;
        public HubConnection HubConnection { get; private set; }

        public GameSocketConnectionManager(GameSocketServerMessageHandler gameSocketServerMessageHandler, HttpClient httpClient)
        {
            _gameSocketServerMessageHandler = gameSocketServerMessageHandler;
            _httpClient = httpClient;
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

        public async Task StartGameSession(string roomKey, GameSessionStartOptions gameSessionStartOptions)
        {
            await HubConnection.SendAsync("StartGameSession", roomKey, gameSessionStartOptions);
        }

        public async Task PlayAgain(string roomKey)
        {
            await HubConnection.SendAsync("PlayAgain", roomKey);
        }

        public async Task SendAnswer(string roomKey, GameRoundAnswer gameRoundAnswer)
        {
            await HubConnection.SendAsync("AnswerChoice", roomKey, gameRoundAnswer);
        }

        public async Task SendAnswerWithData(string roomKey, GameRoundAnswer gameRoundAnswer)
        {
            // send answer with data via Restfull api
            var message = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($@"https://localhost:5001/api/titlesGame/dataAnswer/{roomKey}"),
                Content = new StringContent(JsonConvert.SerializeObject(gameRoundAnswer, Formatting.Indented), Encoding.UTF8, "application/json"),
            };

            await _httpClient.SendAsync(message);
            Console.WriteLine("sending...");
        }
    }
}