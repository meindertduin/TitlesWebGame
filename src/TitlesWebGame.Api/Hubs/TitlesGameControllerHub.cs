using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TitlesWebGame.Api.Hubs
{
    public class TitlesGameControllerHub : Hub
    {
        public async Task ConnectToRoom(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var message = $"{Context.ConnectionId} has joined the group {groupName}.";
            await Clients.Group(groupName).SendAsync("ConnectionStatusUpdate", message);
        }

        public async Task DisconnectFromRoom(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            var message = $"{Context.ConnectionId} has left the group {groupName}.";
            await Clients.Group(groupName).SendAsync("ConnectionStatusUpdate", message);
        }
    }
}