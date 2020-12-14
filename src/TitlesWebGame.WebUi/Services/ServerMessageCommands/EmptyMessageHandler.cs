using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class EmptyMessageHandler : IServerMessageHandler 
    {
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            
        }
    }
}