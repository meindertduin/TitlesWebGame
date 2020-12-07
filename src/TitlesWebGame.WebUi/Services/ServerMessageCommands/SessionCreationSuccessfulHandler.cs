using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class SessionCreationSuccessfulHandler : IServerMessageHandler
    {
        private readonly GameSessionState _gameSessionState;

        public SessionCreationSuccessfulHandler(GameSessionState gameSessionSateManager)
        {
            _gameSessionState = gameSessionSateManager;
        }
        
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            // Todo: initialize game session state here
        }
    }
}