using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Controllers.Hubs;
using MixyBoos.Api.Data.DTO;
using MixyBoos.Api.Data.Models;

namespace MixyBoos.Api.Controllers {
  [Authorize]
  [Route("[controller]")]
  public class DebugController : _Controller {
    private readonly UserManager<MixyBoosUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IHubContext<DebugHub> _hub;

    public DebugController(IConfiguration configuration, ILogger<DebugController> logger, IHubContext<DebugHub> hub,
      UserManager<MixyBoosUser> userManager) :
      base(logger) {
      _configuration = configuration;
      _hub = hub;
      _userManager = userManager;
    }

    [HttpGet("ping")]
    public ActionResult Ping() {
      return Ok($"Pong{Environment.NewLine}");
    }

    [HttpGet]
    public async Task<DebugDTO> GetOsInfo() {
      var config = _configuration.AsEnumerable().ToDictionary(k => k.Key, v => v.Value);
      return await Task.FromResult(new DebugDTO {
        LibVersion = Assembly
          .GetEntryAssembly()?
          .GetCustomAttribute<TargetFrameworkAttribute>()?
          .FrameworkName,
        OSVersion = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
        Config = config
      });
    }

    [HttpGet("sendhub")]
    public async Task<IActionResult> SendHubMessage() {
      var user = await _userManager.FindByNameAsync(User.Identity.Name);
      if (user is null) {
        return Unauthorized();
      }

      await _hub.Clients.User(user.Email)
        .SendAsync("Debuggles", "I'm a little teapot", "Fucking loads of tae");
      return Ok();
    }
  }
}
