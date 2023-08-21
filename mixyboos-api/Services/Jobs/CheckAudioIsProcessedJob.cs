using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MixyBoos.Api.Controllers.Hubs;
using MixyBoos.Api.Data;
using Quartz;

namespace MixyBoos.Api.Services.Jobs;

public class CheckAudioIsProcessedJob : IJob {
  private readonly MixyBoosContext _context;
  private readonly IHubContext<LiveHub> _hub;
  private readonly IConfiguration _config;
  private readonly IHttpClientFactory _httpClientFactory;

  public CheckAudioIsProcessedJob(MixyBoosContext context, IHubContext<LiveHub> hub,
    IConfiguration config, IHttpClientFactory httpClientFactory) {
    _context = context;
    _hub = hub;
    _config = config;
    _httpClientFactory = httpClientFactory;
  }

  public async Task Execute(IJobExecutionContext context) {
    var unprocessed = await _context.Mixes.Where(m => !m.IsProcessed)
      .Include(m => m.User)
      .ToListAsync();

    foreach (var mix in unprocessed) {
      var manifestFile = Path.Combine(_config["AudioProcessing:OutputDir"],
        mix.Id.ToString(),
        $"{mix.Id}.m3u8");
      if (File.Exists(manifestFile)) {
        mix.IsProcessed = true;
      }

      await _hub.Clients.User(mix.User.Email).SendAsync("ConversionFinished", mix.Id);
    }

    await _context.SaveChangesAsync();
  }
}
