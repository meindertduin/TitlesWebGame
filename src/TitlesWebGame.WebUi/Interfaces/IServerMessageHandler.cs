using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public interface IServerMessageHandler
    {
        void Execute(TitlesGameHubMessageModel hubMessageModel);
    }
}