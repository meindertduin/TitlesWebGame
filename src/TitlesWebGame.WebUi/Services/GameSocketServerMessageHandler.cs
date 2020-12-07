using System;
using TitlesWebGame.Domain.Enums;
using TitlesWebGame.Domain.ViewModels;
using TitlesWebGame.WebUi.Services.ServerMessageCommands;

namespace TitlesWebGame.WebUi.Services
{
    public class GameSocketServerMessageHandler
    {
        private readonly GameSessionState _gameSessionState;
        
        public GameSocketServerMessageHandler(GameSessionState gameSessionState)
        {
            _gameSessionState = gameSessionState;
        }
        
        public void Handle(TitlesGameHubMessageModel serverMessage)
        {
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
                _ => throw new ArgumentException(message: "invalid enum value", paramName: nameof(serverMessage.MessageType)),
            };
    }
}