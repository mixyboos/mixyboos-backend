using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OpenIddict.Validation.AspNetCore;

namespace MixyBoos.Api.Controllers.Hubs;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class UpdatesHub : Hub {
    public async Task SendMessage(string userId, string message) {
        await Clients.User(userId).SendAsync("ReceiveMessage", message);
    }
}
