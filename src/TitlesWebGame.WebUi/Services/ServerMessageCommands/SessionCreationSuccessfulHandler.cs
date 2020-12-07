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
            if (hubMessageModel.AppendedObject != null)
            {
                var gameSessionInitModel = hubMessageModel.AppendedObject as GameSessionInitViewModel;
                _gameSessionState.InitializeNewState(gameSessionInitModel);
            }
        }
    }
}