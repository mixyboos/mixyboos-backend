using System;
using System.IO;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Services.Helpers.IO;

namespace MixyBoos.Api.Services.Helpers.Audio;

public class WaveformGenerator {
  private readonly Logger<WaveformGenerator> _logger;

  public WaveformGenerator(Logger<WaveformGenerator> logger) {
    _logger = logger;
  }

  public async Task<(string, string, string)> GenerateWaveformFromFile(string file) {
    _logger.LogInformation("Generating waveform for {LocalFile}", file);
    var datFile = PathHelpers.GetScopedTempFile("dat");
    var jsonFile = PathHelpers.GetScopedTempFile("json");
    var pngFile = PathHelpers.GetScopedTempFile("png");

    var command = Cli.Wrap("audiowaveform");
    _logger.LogInformation("Command is {Command}", command.ToString());
    var datResult = await command
      .WithArguments($"-i {file} -o {datFile} -b 8")
      .ExecuteBufferedAsync();
    _logger.LogInformation("DAT result is {DatResultStandardOutput}", datResult.StandardOutput);

    var jsonArgs = $"-i {file} -o {jsonFile} --pixels-per-second 3 -b 8";
    _logger.LogInformation("JSON args {JsonArgs}", jsonArgs);
    var jsonResult = await command
      .WithArguments(jsonArgs)
      .ExecuteBufferedAsync();
    _logger.LogInformation("JSON result is {JsonResultStandardOutput}", jsonResult.StandardOutput);

    try {
      var pngResult = await command
        .WithArguments(
          $"-i {file} -o {pngFile} -b 8 --no-axis-labels --colors audition --waveform-color baacf1FF --background-color 00000000")
        .ExecuteBufferedAsync();
      _logger.LogInformation("PNG result is {JsonResultStandardOutput}", jsonResult.StandardOutput);
      _logger.LogInformation("PNG error is {JsonResultStandardError}", jsonResult.StandardError);
    } catch (Exception e) {
      _logger.LogDebug("{Message}", e.Message);
    }

    return (
      File.Exists(datFile) ? datFile : string.Empty,
      File.Exists(jsonFile) ? jsonFile : string.Empty,
      File.Exists(pngFile) ? pngFile : string.Empty
    );
  }
}
