using System;
using System.Reflection.PortableExecutable;
using Newtonsoft.Json;
using TitlesWebGame.Domain.ViewModels;

namespace TitlesWebGame.WebUi.Services.ServerMessageCommands
{
    public class NewOwnerMessageHandler : IServerMessageHandler
    {
        private readonly GameSessionState _gameSessionState;

        public NewOwnerMessageHandler(GameSessionState gameSessionState)
        {
            _gameSessionState = gameSessionState;
        }
        public void Execute(TitlesGameHubMessageModel hubMessageModel)
        {
            var newOwnerConnectionId = hubMessageModel.AppendedObject.ToString();
            
            if (String.IsNullOrEmpty(newOwnerConnectionId) == false)
            {
                _gameSessionState.SetNewOwner(newOwnerConnectionId);
            }
        }
    }
}