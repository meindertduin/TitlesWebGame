using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.Api.Services
{
    public interface ITitlesGameHubMessageFactory
    {
        TitlesGameHubMessageModel CreateCreationRoomSuccessfulMessage(GameSessionInitViewModel gameSessionInitState);
        TitlesGameHubMessageModel CreatePlayerJoinedRoomMessage(GameSessionPlayer newPlayerModel);
        TitlesGameHubMessageModel CreateSuccessfullyJoinedMessage(GameSessionInitViewModel gameSessionInitState);
        TitlesGameHubMessageModel CreateErrorConnectingToRoomMessage();
        TitlesGameHubMessageModel CreatePlayerLeftRoomMessage(string displayName, string connectionId);
        TitlesGameHubMessageModel CreateAnswerSuccessfullyProcessedMessage();
        TitlesGameHubMessageModel CreateAnswerTooLateMessage();
        TitlesGameHubMessageModel CreateServerErrorMessage();
        TitlesGameHubMessageModel CreateSessionStartedMessage(int startingAfterDelay);
        TitlesGameHubMessageModel CreatePreviousRoundInfoMessage(SessionStateUpdateViewModel previousRoundInfo);
        TitlesGameHubMessageModel CreateNextRoundInfoMessage(GameRoundInfoViewModel multipleChoiceRoundInfo);
        TitlesGameHubMessageModel CreateEndSessionMessage(TitlesGameEndSessionResults endSessionResults);

        TitlesGameHubMessageModel CreateEndTitlesRoundMessage(TitlesRoundResults titlesRoundResults);
    }
}