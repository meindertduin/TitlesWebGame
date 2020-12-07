namespace TitlesWebGame.Domain.Enums
{
    public enum GameHubMessageType
    {
        GeneralGroup = 0,
        SessionCreationSuccessful = 1,
        PlayerJoinedGroup = 2,
        PlayerLeftGroup = 3,
        AnswerSuccessfullyProcessed = 4,
        AnswerTooLate = 5,
        ErrorConnectingToRoom = 6,
        ErrorCreatingRoom = 7,
        ErrorStartingRoom = 8,
        ServerError = 9,
    }
}