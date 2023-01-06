using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OpenIddict.Validation.AspNetCore;

namespace MixyBoos.Api.Controllers.Hubs;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class ChatHub : Hub {
    public async Task SendMessage(string user, string message, string show, bool join = false) {
        if (join) {
            await JoinRoom(show).ConfigureAwait(false);
            await Clients.Group(show).SendAsync("AckJoin", user, show).ConfigureAwait(true);
        } else {
            await Clients.Group(show).SendAsync("ReceiveMessage", user, message).ConfigureAwait(true);
        }
    }

    public Task JoinRoom(string roomName) {
        return Groups.AddToGroupAsync(Context.ConnectionId, roomName);
    }

    public Task LeaveRoom(string roomName) {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
    }
}
