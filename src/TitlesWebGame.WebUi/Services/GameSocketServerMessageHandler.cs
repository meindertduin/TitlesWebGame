using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

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
            switch (serverMessage.MessageCode)
            {
                case 100:
                    // general group message
                    break;
                case 101:
                    // session creation successful : roomKey
                    _gameSessionSateManager.SetGame(serverMessage.Message);
                    break;
                case 102:
                    // player joined group message
                    break;
                case 103:
                    // player left group message
                    break;
                case 150:
                    // answer successfully processed
                    break;
                case 151:
                    // answer was given too late
                    break;
                case 200:
                    // error connecting to room
                    break;
                case 201:
                    // could not create room
                    break;
                case 202:
                    // could not start room
                    break;
                case 500:
                    // server error
                    break;
            }
        }
    }
}