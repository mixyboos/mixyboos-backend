using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.Models;
using MixyBoos.Api.Services;
using MixyBoos.Api.Services.Helpers.IO;
using Quartz;

namespace MixyBoos.Api.Controllers;

[Authorize]
[Route("[controller]")]
public class JobController(
  ILogger<JobController> logger,
  UserManager<MixyBoosUser> userManager,
  MixyBoosContext context,
  ISchedulerFactory schedulerFactory)
  : _Controller(userManager, logger) {
  [HttpPost("requeuemix")]
  public async Task<IActionResult> RequeueProcessMix([FromQuery] string mixId) {
    if (string.IsNullOrEmpty(mixId)) {
      return BadRequest();
    }

    var localFile = FileHelpers.GetFirstMatchingFile(Constants.TempFolder, mixId);
    var mix = await context.Mixes.FirstOrDefaultAsync(m => m.Id.Equals(Guid.Parse(mixId)));
    if (mix is null ||
        string.IsNullOrEmpty(localFile) ||
        !System.IO.File.Exists(localFile)) {
      return NotFound();
    }

    var jobData = new Dictionary<string, string> {
      {"Id", mixId},
      {"FileLocation", localFile},
      {"UserId", User.Identity.Name}
    };
    var scheduler = await schedulerFactory.GetScheduler();
    await scheduler.TriggerJob(
      new JobKey("ProcessUploadedAudioJob"),
      new JobDataMap(jobData));
    return Ok();
  }
}
