using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Controllers.Hubs;
using MixyBoos.Api.Data;
using Quartz;

namespace MixyBoos.Api.Services.Jobs;

public class CheckAudioIsProcessedJob : IJob {
  private readonly IConfiguration _config;
  private readonly MixyBoosContext _context;
  private readonly IHttpClientFactory _httpClientFactory;
  private readonly ILogger<CheckAudioIsProcessedJob> _logger;
  private readonly IHubContext<LiveHub> _hub;
  private readonly ISchedulerFactory _schedulerFactory;

  public CheckAudioIsProcessedJob(MixyBoosContext context, IHubContext<LiveHub> hub,
    ISchedulerFactory schedulerFactory, IConfiguration config,
    IHttpClientFactory httpClientFactory, ILogger<CheckAudioIsProcessedJob> logger) {
    _context = context;
    _hub = hub;
    _schedulerFactory = schedulerFactory;
    _config = config;
    _httpClientFactory = httpClientFactory;
    _logger = logger;
  }

  public async Task Execute(IJobExecutionContext context) {
    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") {
      return;
    }

    _logger.LogDebug($"Checking for unprocessed audio");

    try {
      var unprocessed = await _context.Mixes.Where(m => !m.IsProcessed)
        .Include(m => m.User)
        .ToListAsync();

      _logger.LogDebug("{UnprocessedCount} unprocessed audio found", unprocessed.Count);
      foreach (var mix in unprocessed) {
        _logger.LogDebug("Reprocessing {mix.Id}", mix.User.Id);
        var manifestFile = Path.Combine(_config["AudioProcessing:OutputDir"],
          mix.Id.ToString(),
          $"{mix.Id}.m3u8");

        _logger.LogDebug("Checking manifest file {manifestFile}", mix.User.Id);
        if (File.Exists(manifestFile)) {
          mix.IsProcessed = true;
        } else {
          var jobData = new Dictionary<string, string> {
            {"Id", mix.Id.ToString()},
            {"FileLocation", mix.__localfile},
            {"UserId", mix.User.Id.ToString()}
          };
          var scheduler = await _schedulerFactory.GetScheduler();
          await scheduler.TriggerJob(
            new JobKey("ProcessUploadedAudioJob"),
            new JobDataMap(jobData));
        }

        await _hub.Clients.User(mix.User.Email).SendAsync("ConversionFinished", mix.Id);
      }

      await _context.SaveChangesAsync();
    } catch (Exception e) {
      _logger.LogError(e, "An error occured while checking for audio");
    }
  }
}
