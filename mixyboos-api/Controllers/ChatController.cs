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
    private readonly UserManager<MixyBoosUser> _userManager;
    private readonly MixyBoosContext _context;
    private readonly IConfiguration _config;
    private readonly IHubContext<ChatHub> _hub;

    public ChatController(
        UserManager<MixyBoosUser> userManager,
        MixyBoosContext context,
        IConfiguration config,
        IHubContext<ChatHub> hub,
        ILogger<ChatController> logger) : base(logger) {
        _userManager = userManager;
        _context = context;
        _config = config;
        _hub = hub;
    }
}
