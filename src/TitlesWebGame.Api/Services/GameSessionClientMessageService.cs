using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using TitlesWebGame.Api.Hubs;
using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Services
{
    public class GameSessionClientMessageService : IGameSessionClientMessageService
    {
        private readonly IHubContext<TitlesGameHub> _titlesGameHub;
        private readonly ITitlesGameHubMessageFactory _titlesGameHubMessageFactory;

        public GameSessionClientMessageService(IHubContext<TitlesGameHub> titlesGameHub,
            ITitlesGameHubMessageFactory titlesGameHubMessageFactory)
        {
            _titlesGameHub = titlesGameHub;
            _titlesGameHubMessageFactory = titlesGameHubMessageFactory;
        }
        
        public Task UpdatePlayersOfNewRoundInfo(string roomKey, GameRoundInfoViewModel gameRoundInfo)
        {
            if (gameRoundInfo != null)
            {
                return _titlesGameHub.Clients.Group(roomKey).SendAsync("ServerMessageUpdate", 
                    _titlesGameHubMessageFactory.CreateNextRoundInfoMessage(gameRoundInfo));
            }
            else
            {
                throw new ArgumentNullException(nameof(gameRoundInfo));
            }
        }
        
        public Task UpdatePlayersOfGameStated(string roomKey)
        {
            int startingAfterDelay = 1;

            return _titlesGameHub.Clients.Group(roomKey).SendAsync("ServerMessageUpdate",
                _titlesGameHubMessageFactory.CreateSessionStartedMessage(startingAfterDelay));
        }
        
        public Task UpdatePlayersOfEndGame(string roomKey, List<GameSessionPlayer> players)
        {
            var endGameResults = new TitlesGameEndSessionResults()
            {
                GameSessionPlayers = players,
            };

            return _titlesGameHub.Clients.Group(roomKey).SendAsync("ServerMessageUpdate",
                _titlesGameHubMessageFactory.CreateEndSessionMessage(endGameResults));
        }
        
        public Task UpdatePlayersOfPreviousRoundInfo(string roomKey, GameRoundInfo previousRoundInfo)
        {
            return _titlesGameHub.Clients.Group(roomKey).SendAsync("ServerMessageUpdate",
                _titlesGameHubMessageFactory.CreatePreviousRoundInfoMessage(previousRoundInfo));
        }
        
        public Task UpdatePlayersOfRoundReview(string roomKey, List<GameSessionPlayer> gameSessionPlayers)
        {
            var previousRoundInfo = new RoundReviewMessageModel()
            {
                GameSessionPlayers = gameSessionPlayers,
            };
            
            return _titlesGameHub.Clients.Group(roomKey).SendAsync("ServerMessageUpdate", 
                _titlesGameHubMessageFactory.CreateRoundReviewMessage(previousRoundInfo));
        }
        
        public Task UpdatePlayersOfTitlesRoundEnded(string roomKey, List<GameSessionPlayer> players)
        {
            var titlesGameRoundResults = new TitlesRoundResults()
            {
                Players = players,
            };
            
            return _titlesGameHub.Clients.Group(roomKey).SendAsync("ServerMessageUpdate", 
                _titlesGameHubMessageFactory.CreateEndTitlesRoundMessage(titlesGameRoundResults));
        }
    }

    public interface IGameSessionClientMessageService
    {
        Task UpdatePlayersOfGameStated(string roomKey);
        Task UpdatePlayersOfEndGame(string roomKey, List<GameSessionPlayer> players);
        Task UpdatePlayersOfPreviousRoundInfo(string roomKey, GameRoundInfo previousRoundInfo);
        Task UpdatePlayersOfRoundReview(string roomKey, List<GameSessionPlayer> gameSessionPlayers);
        Task UpdatePlayersOfTitlesRoundEnded(string roomKey, List<GameSessionPlayer> players);
        Task UpdatePlayersOfNewRoundInfo(string roomKey, GameRoundInfoViewModel gameRoundInfo);
    }
}