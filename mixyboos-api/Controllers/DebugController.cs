using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Controllers.Hubs;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.DTO;
using MixyBoos.Api.Data.Models;
using OpenIddict.Validation.AspNetCore;

namespace MixyBoos.Api.Controllers {
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class DebugController : _Controller {
        private readonly UserManager<MixyBoosUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<DebugHub> _hub;

        public DebugController(UserManager<MixyBoosUser> userManager, IConfiguration configuration,
            ILogger<DebugController> logger,
            IHubContext<DebugHub> hub) : base(logger) {
            _userManager = userManager;
            _configuration = configuration;
            _hub = hub;
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
