using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MixyBoos.Api.Controllers.Hubs;

[Authorize]
public class DebugHub : Hub { }
