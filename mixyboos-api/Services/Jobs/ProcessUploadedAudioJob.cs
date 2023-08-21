using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using FFMpegCore;
using FFMpegCore.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;
using MixyBoos.Api.Controllers.Hubs;
using MixyBoos.Api.Data;
using Quartz;

namespace MixyBoos.Api.Services.Jobs;

// ReSharper disable once ClassNeverInstantiated.Global
public class ProcessUploadedAudioJob : IJob {
  private readonly MixyBoosContext _context;
  private readonly IHubContext<UpdatesHub> _hub;
  private readonly ILogger<ProcessUploadedAudioJob> _logger;
  private readonly IConfiguration _config;

  public ProcessUploadedAudioJob(MixyBoosContext context,
    IHubContext<UpdatesHub> hub,
    ILogger<ProcessUploadedAudioJob> logger,
    IConfiguration config) {
    _context = context;
    _hub = hub;
    _logger = logger;
    _config = config;
  }

  public async Task Execute(IJobExecutionContext context) {
    var data = context.Trigger.JobDataMap;
    var showId = data["Id"]?.ToString();
    var userId = data["UserId"]?.ToString();
    var inputFile = data["FileLocation"]?.ToString();
    var outputPath = _config["AudioProcessing:OutputDir"];

    if (userId is null) {
      _logger.LogError("Error processing {Id} - invalid user id", showId);
      return;
    }

    if (string.IsNullOrEmpty(showId)) {
      _logger.LogError("Error processing {Id} - invalid id", showId);
      return;
    }

    if (!File.Exists(inputFile)) {
      _logger.LogError("Error processing {Id} - unable to locate file {InputFile}", showId, inputFile);
      return;
    }

    if (outputPath is null) {
      _logger.LogError("Error processing {Id} - AudioProcessing:OutputDir must be set", showId);
      return;
    }

    var tempProcessingPath = Path.Combine(Path.GetTempPath(), showId!);

    var finalOutputPath = Path.Combine(outputPath, showId);
    if (!Directory.Exists(finalOutputPath)) {
      Directory.CreateDirectory(finalOutputPath);
    }

    var reader = await FFProbe.AnalyseAsync(inputFile);
    TimeSpan duration = reader.Duration;

    Directory.CreateDirectory(tempProcessingPath);

    var progressHandler = new Action<string>(async p => {
      _logger.LogInformation("Progress on encode: {Percentage}", p);
      await _hub.Clients.User(userId).SendAsync("ConversionProgress", showId, p);
    });
    try {
      var command = Cli.Wrap("ffmpeg")
        .WithArguments(args => args
          .Add(new[] {"-i", inputFile})
          .Add(new[] {"-b:a", "320k"})
          .Add(new[] {"-vn", "-ac", "2", "-acodec", "aac"})
          .Add(new[] {"-f", "segment"})
          .Add(new[] {"-segment_format", "mpegts"})
          .Add(new[] {"-segment_time", "10"})
          .Add(new[] {"-segment_list", $"{Path.Combine(finalOutputPath, showId)}.m3u8"})
          .Add($"{Path.Combine(finalOutputPath, showId)}_%05d.ts")
        ).WithStandardOutputPipe(PipeTarget.ToDelegate(progressHandler));

      var result = await command.ExecuteBufferedAsync();
      _logger.LogInformation("Completed conversion: {Result}", result.ExitCode);

      if (context.CancellationToken.IsCancellationRequested) return;

      //TODO: What if they haven't created the mix yet in the web ui?
      var mix = await _context.Mixes.FirstOrDefaultAsync(m => m.Id.Equals(Guid.Parse(showId)));
      if (mix is not null) {
        mix.IsProcessed = true;
        await _context.SaveChangesAsync();
      }

      await _hub.Clients.User(userId).SendAsync("ConversionFinished", showId);
      _logger.LogInformation("Finished processing {Id}", showId);
    } catch (Exception e) {
      _logger.LogError("Error processing audio upload {Error}", e.Message);
    }
  }
}
