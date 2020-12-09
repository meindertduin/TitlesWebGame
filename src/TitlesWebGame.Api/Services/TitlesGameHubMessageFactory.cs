using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.Enums;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Services
{
    public class TitlesGameHubMessageFactory : ITitlesGameHubMessageFactory
    {
        public TitlesGameHubMessageModel CreateCreationRoomSuccessfulMessage(GameSessionInitViewModel gameSessionInitState)
        {
            return new TitlesGameHubMessageModel()
            {
                MessageType = GameHubMessageType.SessionCreationSuccessful,
                Error = false,
                Message = "Session successfully created",
                AppendedObject = gameSessionInitState,
            };
        }

        public TitlesGameHubMessageModel CreatePlayerJoinedRoomMessage(GameSessionPlayer newPlayerModel)
        {
            return new TitlesGameHubMessageModel()
            {
                MessageType = GameHubMessageType.PlayerJoinedGroup,
                Error = false,
                Message = $"{newPlayerModel.DisplayName} joined the room",
                AppendedObject = newPlayerModel,
            };
        }

        public TitlesGameHubMessageModel CreateSuccessfullyJoinedMessage(GameSessionInitViewModel gameSessionInitState)
        {
            return new TitlesGameHubMessageModel()
            {
                MessageType = GameHubMessageType.SuccessfullyJoinedRoom,
                Error = false,
                Message = "Successfully joined room",
                AppendedObject = gameSessionInitState,
            };
        }

        public TitlesGameHubMessageModel CreateErrorConnectingToRoomMessage()
        {
            return new TitlesGameHubMessageModel()
            {
                MessageType = GameHubMessageType.ErrorConnectingToRoom,
                Error = true,
                Message = "Could not connect with specified room key. Are your sure it's the right key?",
            };
        }

        public TitlesGameHubMessageModel CreatePlayerLeftRoomMessage(string displayName, string connectionId)
        {
            return new TitlesGameHubMessageModel()
            {
                MessageType = GameHubMessageType.PlayerLeftGroup,
                Error = false,
                Message = displayName,
                AppendedObject = connectionId,
            };
        }

        public TitlesGameHubMessageModel CreateAnswerSuccessfullyProcessedMessage()
        {
            return new TitlesGameHubMessageModel()
            {
                MessageType = GameHubMessageType.AnswerSuccessfullyProcessed,
                Error = false,
                Message = "Your answer has been successfully processed",
            };
        }

        public TitlesGameHubMessageModel CreateAnswerTooLateMessage()
        {
            return new TitlesGameHubMessageModel()
            {
                MessageType = GameHubMessageType.AnswerTooLate,
                Error = false,
                Message = "Answer was given outside the round time",
            };
        }

        public TitlesGameHubMessageModel CreateServerErrorMessage()
        {
            return new TitlesGameHubMessageModel()
            {
                MessageType = GameHubMessageType.ServerError,
                Error = true,
                Message = "Something unexpected happened while processing your request"
            };
        }

        public TitlesGameHubMessageModel CreateSessionStartedMessage(int startingAfterDelay)
        {
            return new TitlesGameHubMessageModel()
            {
                Error = false,
                Message = "Game starting",
                AppendedObject = startingAfterDelay,
                MessageType = GameHubMessageType.SessionStarted,
            };
        }

        public TitlesGameHubMessageModel CreatePreviousRoundInfoMessage(SessionStateUpdateViewModel previousRoundInfo)
        {
            return new TitlesGameHubMessageModel()
            {
                Error = false,
                Message = "Previous round update and state loaded",
                MessageType = GameHubMessageType.PreviousRoundInfo,
                AppendedObject = previousRoundInfo,
            };
        }

        public TitlesGameHubMessageModel CreateNextRoundInfoMessage(GameRoundInfoViewModel multipleChoiceRoundInfo)
        {
            return new TitlesGameHubMessageModel()
            {
                Message = "Next round info",
                MessageType = GameHubMessageType.NextRoundInfo,
                AppendedObject = multipleChoiceRoundInfo,
                Error = false,
            };
        }

        public TitlesGameHubMessageModel CreateEndSessionMessage(TitlesGameEndSessionResults endSessionResults)
        {
            return new TitlesGameHubMessageModel()
            {
                Message = "Game ended",
                Error = false,
                MessageType = GameHubMessageType.SessionEnded,
                AppendedObject = endSessionResults,
            };
        }
    }
}