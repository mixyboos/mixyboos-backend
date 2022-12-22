using System;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
    public class LiveController : _Controller {
        private readonly UserManager<MixyBoosUser> _userManager;
        private readonly MixyBoosContext _context;
        private readonly IConfiguration _config;
        private readonly IHubContext<LiveHub> _hub;

        public LiveController(
            UserManager<MixyBoosUser> userManager,
            MixyBoosContext context,
            IConfiguration config,
            IHubContext<LiveHub> hub,
            ILogger<LiveController> logger) : base(logger) {
            _userManager = userManager;
            _context = context;
            _config = config;
            _hub = hub;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartShow([FromBody] CreateLiveShowDTO show) {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var newShow = new LiveShow {
                Title = show.Title,
                StartDate = DateTime.Now,
                Active = true,
                User = user
            };

            await _context.LiveShows.AddAsync(newShow);
            await _context.SaveChangesAsync();

            return Created(nameof(LiveController), newShow);
        }

        [HttpPost("start_stream")]
        [AllowAnonymous]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> ValidateAndStartStream([FromForm] string name) {
            var show = await _context.LiveShows
                .Include(s => s.User)
                .Where(s => s.User.StreamKey.Equals(name))
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();

            if (show is null) {
                return Unauthorized("Invalid stream key");
            }

            //TODO: this should be in a background job... put it here for now
            await _hub.Clients.All.SendAsync("StreamStatusUpdate", show.Id);
            return Redirect($"{show.Id}");
        }

        [HttpGet("current")]
        [Produces("application/json")]
        public async Task<ActionResult<LiveShowDTO>> GetCurrentShow() {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var show = await _context.LiveShows
                .Where(s => s.User.Equals(user))
                .Where(s => s.Active)
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();

            if (show is null) {
                return NoContent();
            }

            return show.Adapt<LiveShowDTO>();
        }

        [HttpGet("current/{userSlug}")]
        [Produces("application/json")]
        [AllowAnonymous]
        public async Task<ActionResult<LiveShowDTO>> GetCurrentShow(string userSlug) {
            var show = await _context.LiveShows
                .Where(s => s.User.Slug.Equals(userSlug))
                .Where(s => s.Active)
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();

            if (show is null) {
                return NotFound("No active show found for user");
            }

            return show.Adapt<LiveShowDTO>();
        }
    }
}
