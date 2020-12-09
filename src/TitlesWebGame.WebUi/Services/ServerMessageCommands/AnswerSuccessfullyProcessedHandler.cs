using System;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class AnswerSuccessfullyProcessedHandler : IServerMessageHandler
    {
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            Console.WriteLine(hubMessageModel.Message);
        }
    }
}