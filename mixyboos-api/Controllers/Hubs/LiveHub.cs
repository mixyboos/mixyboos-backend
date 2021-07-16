using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MixyBoos.Api.Controllers.Hubs {
    public class LiveHub : Hub {
        public async Task SendMessage(string user, string message) {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
