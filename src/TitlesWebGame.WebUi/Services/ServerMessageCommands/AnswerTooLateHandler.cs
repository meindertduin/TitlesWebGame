using System;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class AnswerTooLateHandler : IServerMessageHandler
    {
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            Console.WriteLine("answer too late");
        }
    }
}