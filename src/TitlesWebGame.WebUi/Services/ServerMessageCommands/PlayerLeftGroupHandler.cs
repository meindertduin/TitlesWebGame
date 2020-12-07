using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class PlayerLeftGroupHandler : IServerMessageHandler
    {
        private readonly GameSessionState _gameSessionState;

        public PlayerLeftGroupHandler(GameSessionState gameSessionSateManager)
        {
            _gameSessionState = gameSessionSateManager;
        }
        
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            if (hubMessageModel.AppendedObject != null)
            {
                var leavingPlayerConnId = hubMessageModel.AppendedObject as string;
                _gameSessionState.RemovePlayer(leavingPlayerConnId);
            }
        }
    }
}