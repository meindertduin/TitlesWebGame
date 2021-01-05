using System;
using TitlesWebGame.Domain.Enums;
using TitlesWebGame.Domain.ViewModels;
using TitlesWebGame.WebUi.Services.ServerMessageCommands;
using TitlesWebGame.WebUi.ViewModels;

namespace TitlesWebGame.WebUi.Services
{
    public class GameSocketServerMessageHandler
    {
        private readonly GameSessionState _gameSessionState;
        private readonly ApplicationViewModel _applicationViewModel;

        public GameSocketServerMessageHandler(GameSessionState gameSessionState, ApplicationViewModel applicationViewModel)
        {
            _gameSessionState = gameSessionState;
            _applicationViewModel = applicationViewModel;
        }
        
        public void Handle(TitlesGameHubMessageModel serverMessage)
        {
            if (serverMessage.Error)
            {
                _applicationViewModel.ErrorMessage = serverMessage.Message;
            }

            GetMessageCommandHandler(serverMessage).Execute(serverMessage);
        }
        
        private IServerMessageHandler GetMessageCommandHandler(TitlesGameHubMessageModel serverMessage) =>
            serverMessage.MessageType switch
            {
                GameHubMessageType.GeneralGroup => new GeneralMessageHandler(),
                GameHubMessageType.SessionCreationSuccessful => new SessionCreationSuccessfulHandler(_gameSessionState),
                GameHubMessageType.PlayerJoinedGroup => new PlayerJoinedGroupHandler(_gameSessionState),
                GameHubMessageType.PlayerLeftGroup => new PlayerLeftGroupHandler(_gameSessionState),
                GameHubMessageType.SuccessfullyJoinedRoom => new SuccessfullyJoinedRoomHandler(_gameSessionState),
                GameHubMessageType.SessionStarted => new SessionStartedHandler(_gameSessionState),
                GameHubMessageType.NextRoundInfo => new NextRoundInfoHandler(_gameSessionState),
                GameHubMessageType.PreviousRoundInfo => new PreviousRoundInfoHandler(_gameSessionState),
                GameHubMessageType.RoundReview => new RoundReviewMessageHandler(_gameSessionState),
                GameHubMessageType.AnswerSuccessfullyProcessed => new AnswerSuccessfullyProcessedHandler(),
                GameHubMessageType.AnswerTooLate => new AnswerTooLateHandler(),
                GameHubMessageType.SessionEnded => new SessionEndedHandler(_gameSessionState),
                GameHubMessageType.TitlesRoundEnded => new TitlesRoundEndedHandler(_gameSessionState),
                GameHubMessageType.RejoiningLobby => new RejoiningLobbyMessageHandler(_gameSessionState),
                GameHubMessageType.NewOwnerAssigned => new NewOwnerMessageHandler(_gameSessionState),
                _ => new EmptyMessageHandler(),
            };
    }
}