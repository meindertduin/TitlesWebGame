namespace TitlesWebGame.Domain.Enums
{
    public enum GameHubMessageType
    {
        GeneralGroup,
        SessionCreationSuccessful,
        PlayerJoinedGroup,
        PlayerLeftGroup,
        AnswerSuccessfullyProcessed,
        AnswerTooLate,
        ErrorCreatingRoom,
        ErrorStartingRoom,
        ServerError,
        SuccessfullyJoinedRoom,
        ErrorConnectingToRoom,
    }
}