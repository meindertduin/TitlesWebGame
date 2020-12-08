using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class SessionStartedHandler : IServerMessageHandler
    {
        private readonly GameSessionState _gameSessionState;

        public SessionStartedHandler(GameSessionState gameSessionState)
        {
            _gameSessionState = gameSessionState;
        }


        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            _gameSessionState.SetPlayingStatus(true);
        }
    }
}