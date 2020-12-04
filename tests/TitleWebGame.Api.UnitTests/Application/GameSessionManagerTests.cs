using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;
using Moq;
using TitlesWebGame.Api.Hubs;
using TitlesWebGame.Api.Models;
using TitlesWebGame.Api.Services;
using Xunit;

namespace TitleWebGame.Api.Tests.Application
{
    public class GameSessionManagerTests
    {
        [Fact]
        public void CreateSession_ValidGameSessionPlayer_ReturnsRoomKeyLengthSix()
        {
            // Arrange
            var gameSessionPlayer = getFakeSessionPlayer();
            var gameSessionManager = GetGameSessionManager();

            var roomKey = gameSessionManager.CreateSession(gameSessionPlayer);
            
            Assert.Equal(6, roomKey.Length);
        }

        [Fact]
        public void DeleteSession_AfterValidCreationOfGameSession_ReturnsTrue()
        {
            var gameSessionPlayer = getFakeSessionPlayer();
            var gameSessionManager = GetGameSessionManager();

            var roomKey = gameSessionManager.CreateSession(gameSessionPlayer);
            var removeResult = gameSessionManager.DeleteSession(roomKey);

            Assert.True(removeResult);
        }

        [Fact]
        public void JoinSession_WhenGameNotStarted_ReturnsTrue()
        {
            var gameSessionManager = GetGameSessionManager();
            var gameSessionOwner = getFakeSessionPlayer();
            var roomKey = gameSessionManager.CreateSession(gameSessionOwner);

            var newPlayer = new GameSessionPlayer()
            {
                ConnectionId = "hasdjasdkja",
                CurrentPoints = 0,
                DisplayName = "James"
            };

            var joinResult = gameSessionManager.JoinSession(roomKey, newPlayer);
            
            Assert.True(joinResult);
        }

        private GameSessionPlayer getFakeSessionPlayer()
        {
            return new GameSessionPlayer()
            {
                ConnectionId = "dhajkdakjdahsd",
                CurrentPoints = 0,
                DisplayName = "Mark",
            };
        }

        private GameSessionManager GetGameSessionManager()
        {
            var mockHubContext = new Mock<IHubContext<TitlesGameHub>>();
            return new GameSessionManager(mockHubContext.Object);
        }
    }
}