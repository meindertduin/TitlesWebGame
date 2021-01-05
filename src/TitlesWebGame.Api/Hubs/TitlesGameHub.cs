using System;
using System.Collections.Concurrent;
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
        private readonly ITitlesGameHubMessageFactory _titlesGameHubMessageFactory;
        
        private const int MaxRoundsPerTitleAmount = 20;
        private const int MaxTitleRoundsPerSessionAmount = 20;

        private static ConcurrentDictionary<string, string> _connectedPlayers = new();
        public TitlesGameHub(IGameSessionManager gameSessionManager, ITitlesGameHubMessageFactory titlesGameHubMessageFactory)
        {
            _gameSessionManager = gameSessionManager;
            _titlesGameHubMessageFactory = titlesGameHubMessageFactory;
        }

        public async Task CreateRoom(string displayName)
        {
            var ownerGameSessionPlayerModel = new GameSessionPlayer()
            {
                DisplayName = displayName,
                ConnectionId = Context.ConnectionId,
                CurrentPoints = 0,
            };
            
            var gameSessionInitState= _gameSessionManager.CreateSession(ownerGameSessionPlayerModel);

            _connectedPlayers.TryAdd(Context.ConnectionId, gameSessionInitState.RoomKey);

            await Clients.Caller.SendAsync("ServerMessageUpdate", 
                _titlesGameHubMessageFactory.CreateCreationRoomSuccessfulMessage(gameSessionInitState));
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
                _connectedPlayers.TryAdd(Context.ConnectionId, joinSessionResult.RoomKey);
                
                await Clients.Group(roomKey).SendAsync("ServerMessageUpdate", 
                    _titlesGameHubMessageFactory.CreatePlayerJoinedRoomMessage(newPlayerModel));
                
                await Groups.AddToGroupAsync(Context.ConnectionId, roomKey);

                await Clients.Caller.SendAsync("ServerMessageUpdate", 
                    _titlesGameHubMessageFactory.CreateSuccessfullyJoinedMessage(joinSessionResult));
            }
            else
            {
                await Clients.Caller.SendAsync("ServerMessageUpdate", 
                    _titlesGameHubMessageFactory.CreateErrorConnectingToRoomMessage());
            }
        }
        public void StartGameSession(string roomKey, GameSessionStartOptions gameSessionStartOptions)
        {
            if (gameSessionStartOptions.TitleRoundsAmount > 0 && gameSessionStartOptions.TitleRoundsAmount <= MaxTitleRoundsPerSessionAmount &&
                gameSessionStartOptions.RoundsPerTitleAmount > 0 && gameSessionStartOptions.RoundsPerTitleAmount <= MaxRoundsPerTitleAmount)   
            {
                _gameSessionManager.StartSession(roomKey, Context.ConnectionId, gameSessionStartOptions);
            }
            else
            {
                Clients.Caller.SendAsync("ServerMessageUpdate",
                    _titlesGameHubMessageFactory.CreateFailedToStartSessionMessage(
                        "Couldn't start session due to invalid options selected"));
            }
        }

        public async Task PlayAgain(string roomKey)
        {
            var canPlayAgain = _gameSessionManager.PlayAgain(roomKey, Context.ConnectionId);
            if (canPlayAgain)
            {
                await Clients.Group(roomKey).SendAsync("ServerMessageUpdate",
                    _titlesGameHubMessageFactory.CreateRejoiningLobbyMessage());
            }
        }
        
        public async Task DisconnectFromRoom(string roomKey, string displayName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomKey);

            var removeResult = _connectedPlayers.TryRemove(Context.ConnectionId, out _);
            if (removeResult)
            {
                _gameSessionManager.LeaveSession(roomKey, Context.ConnectionId);
            }
            
            await Clients.Group(roomKey).SendAsync("ServerMessageUpdate", 
                _titlesGameHubMessageFactory.CreatePlayerLeftRoomMessage(displayName, Context.ConnectionId));
        }
        public async Task AnswerChoice(string roomKey, GameRoundAnswer gameRoundAnswer)
        {
            var answerProcessed = _gameSessionManager.AddAnswer(roomKey, gameRoundAnswer);
            TitlesGameHubMessageModel callerAnswer;
            
            if (answerProcessed)
            {
                callerAnswer = _titlesGameHubMessageFactory.CreateAnswerSuccessfullyProcessedMessage();
            }
            else
            {
                callerAnswer = _titlesGameHubMessageFactory.CreateAnswerTooLateMessage();
            }

            await Clients.Caller.SendAsync("ServerMessageUpdate", callerAnswer);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var removeResult = _connectedPlayers.TryRemove(Context.ConnectionId, out var roomKey);
            if (removeResult)
            {
                _gameSessionManager.LeaveSession(roomKey, Context.ConnectionId);
                await Clients.Group(roomKey).SendAsync("ServerMessageUpdate", 
                    _titlesGameHubMessageFactory.CreatePlayerLeftRoomMessage("player", Context.ConnectionId));
            }

            await base.OnDisconnectedAsync(exception);
        }

        private TitlesGameHubMessageModel GetServerErrorMessageModel()
        {
            return _titlesGameHubMessageFactory.CreateServerErrorMessage();
        }
    }
}