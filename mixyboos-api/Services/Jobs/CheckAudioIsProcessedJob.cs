using System;
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

  public CheckAudioIsProcessedJob(MixyBoosContext context, IHubContext<LiveHub> hub,
    IConfiguration config, IHttpClientFactory httpClientFactory, ILogger<CheckAudioIsProcessedJob> logger) {
    _context = context;
    _hub = hub;
    _config = config;
    _httpClientFactory = httpClientFactory;
    _logger = logger;
  }

  public async Task Execute(IJobExecutionContext context) {
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
        }

        await _hub.Clients.User(mix.User.Email).SendAsync("ConversionFinished", mix.Id);
      }

      await _context.SaveChangesAsync();
    } catch (Exception e) {
      _logger.LogError(e, "An error occured while checking for audio");
    }
  }
}
