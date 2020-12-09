using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class TitlesRoundEnded : IServerMessageHandler
    {
        private readonly GameSessionState _gameSessionState;

        public TitlesRoundEnded(GameSessionState gameSessionState)
        {
            _gameSessionState = gameSessionState;
        }
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            
        }
    }
}