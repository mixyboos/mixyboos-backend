using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Data;
using MixyBoos.Api.Services;
using MixyBoos.Api.Services.Helpers.IO;
using OpenIddict.Validation.AspNetCore;
using Quartz;

namespace MixyBoos.Api.Controllers;

[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[Route("[controller]")]
public class JobController : _Controller {
  private readonly MixyBoosContext _context;
  private readonly ISchedulerFactory _schedulerFactory;

  public JobController(ILogger<JobController> logger,
    MixyBoosContext context, ISchedulerFactory schedulerFactory) :
    base(logger) {
    _context = context;
    _schedulerFactory = schedulerFactory;
  }

  [HttpPost("requeuemix")]
  public async Task<IActionResult> RequeueProcessMix([FromQuery] string mixId) {
    if (string.IsNullOrEmpty(mixId)) {
      return BadRequest();
    }

    var localFile = FileHelpers.GetFirstMatchingFile(Constants.TempFolder, mixId);
    var mix = await _context.Mixes.FirstOrDefaultAsync(m => m.Id.Equals(Guid.Parse(mixId)));
    if (mix is null ||
        string.IsNullOrEmpty(localFile) ||
        !System.IO.File.Exists(localFile)) {
      return NotFound();
    }

    var jobData = new Dictionary<string, string>() {
      {"Id", mixId},
      {"FileLocation", localFile},
      {"UserId", User.Identity.Name}
    };
    var scheduler = await _schedulerFactory.GetScheduler();
    await scheduler.TriggerJob(
      new JobKey("ProcessUploadedAudioJob"),
      new JobDataMap(jobData));
    return Ok();
  }
}
