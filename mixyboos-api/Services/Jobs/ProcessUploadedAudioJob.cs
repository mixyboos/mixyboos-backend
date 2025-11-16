using System;
using System.IO;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using FFMpegCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Controllers.Hubs;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.Models;
using MixyBoos.Api.Services.Helpers.Audio;
using Quartz;

namespace MixyBoos.Api.Services.Jobs;

// ReSharper disable once ClassNeverInstantiated.Global
public class ProcessUploadedAudioJob : IJob {
  private readonly IConfiguration _config;
  private readonly IWaveformGenerator _waveformGenerator;
  private readonly MixyBoosContext _context;
  private readonly IHubContext<UpdatesHub> _hub;
  private readonly ILogger<ProcessUploadedAudioJob> _logger;

  public ProcessUploadedAudioJob(MixyBoosContext context,
    IHubContext<UpdatesHub> hub,
    IConfiguration config,
    IWaveformGenerator waveformGenerator,
    ILogger<ProcessUploadedAudioJob> logger) {
    _context = context;
    _hub = hub;
    _config = config;
    _waveformGenerator = waveformGenerator;
    _logger = logger;
  }

  public async Task Execute(IJobExecutionContext context) {
    var data = context.Trigger.JobDataMap;
    var showId = data["Id"]?.ToString();
    var userId = data["UserId"]?.ToString();
    var inputFile = data["FileLocation"]?.ToString();
    var outputPath = _config["AudioProcessing:OutputDir"];

    var user = await _context
      .Users
      .FirstOrDefaultAsync(u => u.Id.Equals(Guid.Parse(userId)));

    if (userId is null || user?.Email is null) {
      _logger.LogError("Error processing {Id} - invalid user id", showId);
      return;
    }

    try {
      if (string.IsNullOrEmpty(showId)) {
        var errorMsg = "Invalid mix ID provided";
        await _hub.Clients.User(user.Email).SendAsync("ConversionFailed", showId, errorMsg);
        _logger.LogError("Error processing {Id} - invalid id", showId);
        return;
      }

      if (!File.Exists(inputFile)) {
        var errorMsg = $"Unable to locate uploaded file";
        await _hub.Clients.User(user.Email).SendAsync("ConversionFailed", showId, errorMsg);
        _logger.LogError("Error processing {Id} - unable to locate file {InputFile}", showId, inputFile);
        return;
      }

      if (outputPath is null) {
        var errorMsg = "Server configuration error - output directory not set";
        await _hub.Clients.User(user.Email).SendAsync("ConversionFailed", showId, errorMsg);
        _logger.LogError("Error processing {Id} - AudioProcessing:OutputDir must be set", showId);
        return;
      }

      var tempProcessingPath = Path.Combine(Path.GetTempPath(), showId!);

      var finalOutputPath = Path.Combine(outputPath, showId);
      if (!Directory.Exists(finalOutputPath)) {
        Directory.CreateDirectory(finalOutputPath);
      }

      Directory.CreateDirectory(tempProcessingPath);
      await _hub.Clients.User(user.Email).SendAsync("ConversionStarted", showId);

      var waveformProgressHandler = new Action<string>(async void (output) => { });

      var waveformResult = await _waveformGenerator.GenerateWaveformFromFile(
        inputFile,
        showId,
        waveformProgressHandler);

      var ffmpegProgressHandler = new Action<string>(async void (output) => {
        try {
          if (!output.Contains("time=")) {
            return;
          }

          var timeIndex = output.IndexOf("time=", StringComparison.Ordinal);
          if (timeIndex < 0) {
            return;
          }

          var timeStr = output.Substring(timeIndex + 5, 11).Trim();
          if (!TimeSpan.TryParse(timeStr, out var processedTime)) {
            return;
          }

          var audioInfo = await FFProbe.AnalyseAsync(inputFile);
          var totalDuration = audioInfo.Duration;

          var percentage = (int)((processedTime.TotalSeconds / totalDuration.TotalSeconds) * 100);
          percentage = Math.Min(percentage, 100); // Cap at 100%

          _logger.LogInformation("Progress on encode: {Percentage}%", percentage);
          await _hub.Clients.User(user.Email).SendAsync("ConversionProgress", showId, percentage);
        } catch (Exception e) {
          _logger.LogError("Error sending progress {Error}", e);
        }
      });

      var command = Cli.Wrap("ffmpeg")
        .WithArguments(args => args
          .Add(["-i", inputFile])
          .Add(["-b:a", "320k"])
          .Add(["-vn", "-ac", "2", "-acodec", "aac"])
          .Add(["-f", "segment"])
          .Add(["-segment_format", "mpegts"])
          .Add(["-segment_time", "10"])
          .Add(["-segment_list", $"{Path.Combine(finalOutputPath, showId)}.m3u8"])
          .Add($"{Path.Combine(finalOutputPath, showId)}_%05d.ts")
        ).WithStandardErrorPipe(PipeTarget.ToDelegate(ffmpegProgressHandler));

      var result = await command.ExecuteBufferedAsync();
      _logger.LogInformation("Completed conversion: {Result}", result.ExitCode);

      if (context.CancellationToken.IsCancellationRequested) {
        return;
      }

      //TODO: What if they haven't created the mix yet in the web ui?
      var mix = await _context.Mixes.FirstOrDefaultAsync(m => m.Id.Equals(Guid.Parse(showId)));
      if (mix is null) {
        mix = new Mix {
          Id = Guid.Parse(showId),
          Title = $"{showId}",
          Description = $"{showId}",
          User = user
        };
        await _context.Mixes.AddAsync(mix);
      }

      mix.IsProcessed = true;
      mix.Duration = await AudioHelpers.GetAudioDuration(inputFile);

      await _context.SaveChangesAsync();

      await _hub.Clients.User(user.Email).SendAsync("ConversionFinished", showId);
      _logger.LogInformation("Finished processing {Id}", showId);
    } catch (Exception e) {
      _logger.LogError("Error processing audio upload {Error}", e.Message);
      var errorMessage = $"Processing failed: {e.Message}";
      await _hub.Clients.User(user.Email).SendAsync("ConversionFailed", showId, errorMessage);
    }
  }
}
