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
        FailedStartingSession,
        ServerError,
        SuccessfullyJoinedRoom,
        ErrorConnectingToRoom,
        SessionStarted,
        NextRoundInfo,
        PreviousRoundInfo,
        RoundReview,
        SessionEnded,
        TitlesRoundEnded,
        RejoiningLobby,
    }
}