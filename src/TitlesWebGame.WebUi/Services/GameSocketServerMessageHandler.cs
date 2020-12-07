using System;
using TitlesWebGame.Domain.Enums;
using TitlesWebGame.Domain.ViewModels;
using TitlesWebGame.WebUi.Services.ServerMessageCommands;

namespace TitlesWebGame.WebUi.Services
{
    public class GameSocketServerMessageHandler
    {
        private readonly GameSocketConnectionManager _gameSocketConnectionManager;
        private readonly GameSessionSateManager _gameSessionSateManager;
        
        public GameSocketServerMessageHandler(GameSocketConnectionManager gameSocketConnectionManager, 
            GameSessionSateManager gameSessionSateManager)
        {
            _gameSocketConnectionManager = gameSocketConnectionManager;
            _gameSessionSateManager = gameSessionSateManager;
        }
        
        public void Handle(TitlesGameHubMessageModel serverMessage)
        {
            GetMessageCommandHandler(serverMessage).Execute(serverMessage);
        }

        private IServerMessageHandler GetMessageCommandHandler(TitlesGameHubMessageModel serverMessage) =>
            serverMessage.MessageType switch
            {
                GameHubMessageType.GeneralGroup => new GeneralMessageHandler(),
                GameHubMessageType.SessionCreationSuccessful => new SessionCreationSuccessfulHandler(
                    _gameSessionSateManager),
                _ => throw new ArgumentException(message: "invalid enum value", paramName: nameof(serverMessage.MessageType)),
            };
    }
}