using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TitlesWebGame.Api.Models;
using TitlesWebGame.Api.Services;

namespace TitlesWebGame.Api.Hubs
{
    public class TitlesGameHub : Hub
    {
        private readonly IGameSessionManager _gameSessionManager;

        public TitlesGameHub(IGameSessionManager gameSessionManager)
        {
            _gameSessionManager = gameSessionManager;
        }

        public async Task CreateRoom(string displayName)
        {
            var roomKey= _gameSessionManager.CreateSession(new GameSessionPlayer()
            {
                DisplayName = displayName,
                ConnectionId = Context.ConnectionId,
                CurrentPoints = 0,
            });

            await Clients.Caller.SendAsync("ConnectionStatusUpdate", roomKey);
        }
        
        public async Task ConnectToRoom(string roomKey, string displayName)
        {
            var joinSessionResult = _gameSessionManager.JoinSession(roomKey, new GameSessionPlayer()
            {
                DisplayName = displayName,
                ConnectionId = Context.ConnectionId,
                CurrentPoints = 0,
            });
            
            if (joinSessionResult)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roomKey);
                var message = $"{displayName} has joined the group {displayName}.";
                await Clients.Group(roomKey).SendAsync("ConnectionStatusUpdate", message);
            }
            else
            {
                // implement way of notifying client of error joining room
            }
        }

        public async Task StartGameSession(string roomKey)
        {
            _gameSessionManager.StartSession(roomKey, Context.ConnectionId);
        }
        
        public async Task DisconnectFromRoom(string roomKey)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomKey);
            var message = $"{Context.ConnectionId} has left the group {roomKey}.";
            await Clients.Group(roomKey).SendAsync("ConnectionStatusUpdate", message);
        }

        public void AnswerChoice(string roomKey, GameRoundAnswer gameRoundAnswer)
        {
            _gameSessionManager.AddAnswer(roomKey, gameRoundAnswer);
        }
    }
}