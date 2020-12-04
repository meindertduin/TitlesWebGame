using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TitlesWebGame.Api.Models;
using TitlesWebGame.Api.Services;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

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

            await Clients.Caller.SendAsync("ServerMessageUpdate", new TitlesGameHubMessageModel()
            {
                MessageCode = 101,
                Error = false,
                Message = roomKey,
            });
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
                await Clients.Group(roomKey).SendAsync("ServerMessageUpdate", new TitlesGameHubMessageModel()
                {
                    MessageCode = 102,
                    Error = false,
                    Message = displayName,
                });
            }
            else
            {
                await Clients.Caller.SendAsync("ServerMessageUpdate", new TitlesGameHubMessageModel()
                {
                    MessageCode = 200,
                    Error = true,
                    Message = "Could not connect with specified room key. Are your sure it's the right key?",
                });
            }
        }

        public void StartGameSession(string roomKey)
        {
            _gameSessionManager.StartSession(roomKey, Context.ConnectionId);
        }
        
        public async Task DisconnectFromRoom(string roomKey, string displayName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomKey);
            await Clients.Group(roomKey).SendAsync("ServerMessageUpdate", new TitlesGameHubMessageModel()
            {
                MessageCode = 103,
                Error = false,
                Message = displayName,
            });
        }

        public async Task AnswerChoice(string roomKey, GameRoundAnswer gameRoundAnswer)
        {
            var answerProcessed = _gameSessionManager.AddAnswer(roomKey, gameRoundAnswer);
            TitlesGameHubMessageModel callerAnswer = null;
            
            if (answerProcessed)
            {
                callerAnswer = new TitlesGameHubMessageModel()
                {
                    MessageCode = 150,
                    Error = false,
                    Message = "Your answer has been processed",
                };
            }
            else
            {
                callerAnswer = new TitlesGameHubMessageModel()
                {
                    MessageCode = 151,
                    Error = false,
                    Message = "Answer was given outside the round time",
                };
            }

            await Clients.Caller.SendAsync("ServerMessageUpdate", callerAnswer);
        }

        private TitlesGameHubMessageModel GetServerErrorMessageModel()
        {
            return new TitlesGameHubMessageModel()
            {
                MessageCode = 500,
                Error = true,
                Message = "Something unexpected happened while processing your request"
            };
        }
    }
}