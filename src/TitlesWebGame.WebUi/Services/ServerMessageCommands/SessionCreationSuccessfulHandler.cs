using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class SessionCreationSuccessfulHandler : IServerMessageHandler
    {
        private readonly GameSessionSateManager _gameSessionSateManager;

        public SessionCreationSuccessfulHandler(GameSessionSateManager gameSessionSateManager)
        {
            _gameSessionSateManager = gameSessionSateManager;
        }
        
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            _gameSessionSateManager.SetGame(hubMessageModel.Message);
        }
    }
}