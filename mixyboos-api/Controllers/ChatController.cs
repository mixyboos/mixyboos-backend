using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Controllers.Hubs;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.Models;

namespace MixyBoos.Api.Controllers;

[Authorize]
[Route("[controller]")]
public class ChatController : _Controller {
  public ChatController(
    UserManager<MixyBoosUser> userManager,
    MixyBoosContext context,
    IConfiguration config,
    IHubContext<ChatHub> hub,
    ILogger<ChatController> logger) : base(userManager, logger) { }
}
