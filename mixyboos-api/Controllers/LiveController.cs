using System;
using System.Collections.Generic;
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
using MixyBoos.Api.Services.Extensions;
using OpenIddict.Validation.AspNetCore;
using Quartz;

namespace MixyBoos.Api.Controllers {
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class LiveController : _Controller {
        private readonly UserManager<MixyBoosUser> _userManager;
        private readonly MixyBoosContext _context;
        private readonly IConfiguration _config;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IHubContext<LiveHub> _hub;

        public LiveController(
            UserManager<MixyBoosUser> userManager,
            MixyBoosContext context,
            IConfiguration config,
            ISchedulerFactory schedulerFactory,
            IHubContext<LiveHub> hub,
            ILogger<LiveController> logger) : base(logger) {
            _userManager = userManager;
            _context = context;
            _config = config;
            _schedulerFactory = schedulerFactory;
            _hub = hub;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartShow([FromBody] CreateLiveShowDTO show) {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var newShow = new LiveShow {
                Title = show.Title,
                Description = show.Description,
                Tags = await _context.MapTags(show.Tags),
                StartDate = DateTime.UtcNow,
                User = user
            };

            await _context.LiveShows.AddAsync(newShow);
            await _context.SaveChangesAsync();

            return Created(nameof(LiveController), newShow.Adapt<CreateLiveShowDTO>());
        }

        [HttpPost("on_publish")]
        [AllowAnonymous]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> OnPublish([FromForm] string name) {
            //name is StreamKey
            var user = await _userManager.FindByStreamKeyAsync(name);
            if (user is null) {
                return NotFound("Invalid stream key");
            }

            var show = await _context.LiveShows
                .Include(s => s.User)
                .Where(s => s.User.Equals(user))
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();

            if (show is null) {
                return Unauthorized("Invalid stream key");
            }

            if (show.IsFinished) {
                //obviously a stream glitch, let's reopen it and see what's what
                show.IsFinished = false;
                await _context.SaveChangesAsync();
            }

            try {
                var scheduler = await _schedulerFactory.GetScheduler();
                await scheduler.TriggerJob(new JobKey("CheckLiveStreamJob", "DEFAULT"), new JobDataMap(
                    new Dictionary<string, string> {
                        {"UserEmail", user.Email},
                        {"ShowId", show.Id.ToString()}
                    }
                ));
            } catch (Exception e) {
                _logger.LogError("Error starting job {Message}", e.Message);
            }

            // return Redirect($"rtmp://127.0.0.1:1935/hls-live/{show.Id}");
            return Redirect(show.Id.ToString());
        }

        [HttpPost("on_publish_done")]
        [AllowAnonymous]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> OnPublishDone([FromForm] string name) {
            var user = await _userManager.FindByStreamKeyAsync(name);
            if (user is null) {
                return NotFound("Invalid stream key");
            }

            var show = await _context.LiveShows
                .Include(s => s.User)
                .Where(s => s.User.Equals(user))
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();
            if (show is null) {
                return Unauthorized("Invalid stream key");
            }

            show.IsFinished = true;
            await _context.SaveChangesAsync();
            try {
                var scheduler = await _schedulerFactory.GetScheduler();
                await scheduler.TriggerJob(new JobKey("SaveLiveShowJob", "DEFAULT"), new JobDataMap(
                    new Dictionary<string, string> {
                        {"ShowId", show.Id.ToString()}
                    }
                ));
            } catch (Exception e) {
                _logger.LogError("Error starting job {Message}", e.Message);
            }

            await _hub.Clients.User(user.Email).SendAsync("StreamEnded", show.Id);
            return Ok();
        }

        [HttpGet("current")]
        [Produces("application/json")]
        public async Task<ActionResult<LiveShowDTO>> GetCurrentShow() {
            if (User.Identity is not {Name: { }}) {
                return Unauthorized();
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var show = await _context.LiveShows
                .Where(s => s.User.Equals(user))
                .Where(s => !s.IsFinished)
                .Include(s => s.Tags)
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
                .Where(s => s.IsFinished)
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();

            if (show is null) {
                return NotFound("No active show found for user");
            }

            return show.Adapt<LiveShowDTO>();
        }
    }
}
