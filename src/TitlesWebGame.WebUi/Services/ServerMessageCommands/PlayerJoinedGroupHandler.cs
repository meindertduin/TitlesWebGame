using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class PlayerJoinedGroupHandler : IServerMessageHandler
    {
        private readonly GameSessionSateManager _gameSessionSateManager;

        public PlayerJoinedGroupHandler(GameSessionSateManager gameSessionSateManager)
        {
            _gameSessionSateManager = gameSessionSateManager;
        }
        
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            if (hubMessageModel.AppendedObject != null)
            {
                var joinedPlayer = hubMessageModel.AppendedObject as GameSessionPlayer;
                _gameSessionSateManager.AddPlayer(joinedPlayer);
            }
        }
    }
}