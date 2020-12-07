using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TitlesWebGame.Api.Models;
using TitlesWebGame.Api.Services;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.Enums;
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
                MessageType = GameHubMessageType.SessionCreationSuccessful,
                Error = false,
                Message = roomKey,
            });
        }
        
        public async Task ConnectToRoom(string roomKey, string displayName)
        {
            var newPlayerModel = new GameSessionPlayer()
            {
                DisplayName = displayName,
                ConnectionId = Context.ConnectionId,
                CurrentPoints = 0,
            };
            
            var joinSessionResult = _gameSessionManager.JoinSession(roomKey, newPlayerModel);
            
            if (joinSessionResult != null)
            {
                await Clients.Group(roomKey).SendAsync("ServerMessageUpdate", new TitlesGameHubMessageModel()
                {
                    MessageType = GameHubMessageType.PlayerJoinedGroup,
                    Error = false,
                    Message = $"{displayName} joined the room",
                    AppendedObject = newPlayerModel,
                });
                
                await Groups.AddToGroupAsync(Context.ConnectionId, roomKey);

                await Clients.Caller.SendAsync("ServerMessageUpdate", new TitlesGameHubMessageModel()
                {
                    MessageType = GameHubMessageType.SuccessfullyJoinedRoom,
                    Error = false,
                    Message = "Successfully joined room",
                    AppendedObject = joinSessionResult,
                });
            }
            else
            {
                await Clients.Caller.SendAsync("ServerMessageUpdate", new TitlesGameHubMessageModel()
                {
                    MessageType = GameHubMessageType.ErrorConnectingToRoom,
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
                MessageType = GameHubMessageType.PlayerLeftGroup,
                Error = false,
                Message = displayName,
                AppendedObject = Context.ConnectionId,
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
                    MessageType = GameHubMessageType.AnswerSuccessfullyProcessed,
                    Error = false,
                    Message = "Your answer has been processed",
                };
            }
            else
            {
                callerAnswer = new TitlesGameHubMessageModel()
                {
                    MessageType = GameHubMessageType.AnswerTooLate,
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
                MessageType = GameHubMessageType.ServerError,
                Error = true,
                Message = "Something unexpected happened while processing your request"
            };
        }
    }
}