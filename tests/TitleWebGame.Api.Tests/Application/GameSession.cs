using System.Threading.Tasks;
using TitlesWebGame.Api.Models;
using Xunit;

namespace TitleWebGame.Api.Tests.Application
{
    public class GameSession
    {
        [Fact]
        public async Task PlayRound_MultipleChoiceRound_AddTwoAnswers_ReturnsCountTwo()
        {
            // arrange
            var roundInfo = GetMultipleChoiceRoundInfo();
            
            var gameRound = new MultipleChoiceGameRound(roundInfo.Answer, roundInfo.RewardPoints, roundInfo.RoundTimeMs);

            var gameAnswerOne = new MultipleChoiceAnswer()
            {
                ConnectionId = "jbasdkjaskjha",
                Answer = 1,
            };
            
            var gameAnswerTwo = new MultipleChoiceAnswer()
            {
                ConnectionId = "asdasdvcasdas",
                Answer = 2,
            };
            
            // act
            var playRoundTask = gameRound.PlayRound();
            gameRound.AddAnswer(gameAnswerOne);
            gameRound.AddAnswer(gameAnswerTwo);
            var scores = await playRoundTask;
            
            // Assert
            
            Assert.True(scores.Count == 2);
        }

        [Fact]
        public async Task AddAnswer_MultipleChoiceRound_AddAnswerAfterRoundTimeOver_ReturnsFalse()
        {
            // Arrange
            var roundInfo = GetMultipleChoiceRoundInfo();
            var gameRound = new MultipleChoiceGameRound(roundInfo.Answer, roundInfo.RewardPoints, roundInfo.RoundTimeMs);

            var gameAnswerOne = new MultipleChoiceAnswer()
            {
                ConnectionId = "jbasdkjaskjha",
                Answer = 1,
            };
            
            // Act
            await gameRound.PlayRound();
            var addResult = gameRound.AddAnswer(gameAnswerOne);
            
            // Assert
            Assert.False(addResult);
        }

        [Fact]
        public async Task AddAnswer_MultipleChoiceRound_AddAnswerBeforeRoundTimeOver_ReturnsTrue()
        {
            // Arrange
            var roundInfo = GetMultipleChoiceRoundInfo();
            var gameRound = new MultipleChoiceGameRound(roundInfo.Answer, roundInfo.RewardPoints, roundInfo.RoundTimeMs);
            
            var gameAnswerOne = new MultipleChoiceAnswer()
            {
                ConnectionId = "jbasdkjaskjha",
                Answer = 1,
            };
            
            // Act
            var playRoundTask = gameRound.PlayRound();
            var addResult = gameRound.AddAnswer(gameAnswerOne);
            await playRoundTask;
            
            // Assert
            Assert.True(addResult);

        }

        private MultipleChoiceRoundInfo GetMultipleChoiceRoundInfo()
        {
            return new()
            {
                Answer = 1,
                Choices = new []{ "bear", "zebra", "giraffe", "crocodile"},
                RewardPoints = 500,
                GameRoundsType = GameRoundsType.MultipleChoiceRound,
                RoundStatement = "What animal is primarily known for having stripes",
                RoundTimeMs = 100,
            };
        }
    }
}