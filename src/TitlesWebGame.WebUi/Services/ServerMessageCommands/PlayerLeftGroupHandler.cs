using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class PlayerLeftGroupHandler : IServerMessageHandler
    {
        private readonly GameSessionSateManager _gameSessionSateManager;

        public PlayerLeftGroupHandler(GameSessionSateManager gameSessionSateManager)
        {
            _gameSessionSateManager = gameSessionSateManager;
        }
        
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            if (hubMessageModel.AppendedObject != null)
            {
                var leavingPlayerConnId = hubMessageModel.AppendedObject as string;
                _gameSessionSateManager.RemovePlayer(leavingPlayerConnId);
            }
        }
    }
}