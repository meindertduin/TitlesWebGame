using TitlesWebGame.Domain.Entities;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class PlayerJoinedGroupHandler : IServerMessageHandler
    {
        private readonly GameSessionState _gameSessionState;

        public PlayerJoinedGroupHandler(GameSessionState gameSessionSateManager)
        {
            _gameSessionState = gameSessionSateManager;
        }
        
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            if (hubMessageModel.AppendedObject != null)
            {
                var joinedPlayer = hubMessageModel.AppendedObject as GameSessionPlayer;
                _gameSessionState.AddPlayer(joinedPlayer);
            }
        }
    }
}