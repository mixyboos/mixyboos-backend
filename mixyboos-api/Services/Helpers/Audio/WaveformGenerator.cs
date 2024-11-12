using System;
using System.IO;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Services.Helpers.IO;

namespace MixyBoos.Api.Services.Helpers.Audio;

public interface IWaveformGenerator {
  Task<(string, string, string)> GenerateWaveformFromFile(string file, string id);
}

public class WaveformGenerator : IWaveformGenerator {
  private readonly ILogger<WaveformGenerator> _logger;
  private readonly IConfiguration _config;

  public WaveformGenerator(ILogger<WaveformGenerator> logger, IConfiguration config) {
    _logger = logger;
    _config = config;
  }

  public async Task<(string, string, string)> GenerateWaveformFromFile(string file, string id) {
    _logger.LogInformation("Generating waveform for {LocalFile}", file);
    //TODO: Move config to POCO's to avoid "Possible null assignments"
    var outputDir = Path.Combine(_config["AudioProcessing:WaveformDir"], id);
    if (string.IsNullOrEmpty(outputDir)) {
      _logger.LogError("Output directory is missing");
      throw new InvalidOperationException("Waveform output directory is missing");
    }

    if (!Directory.Exists(outputDir)) {
      Directory.CreateDirectory(outputDir);
    }

    var tmpDatFile = PathHelpers.GetScopedTempFile("dat");
    var tmpJsonFile = PathHelpers.GetScopedTempFile("json");
    var tmpPngFile = PathHelpers.GetScopedTempFile("png");

    string outputDatFile = Path.Combine(outputDir, $"{id}.dat");
    string outputJsonFile = Path.Combine(outputDir, $"{id}.json");
    string outputPngFile = Path.Combine(outputDir, $"{id}.png");

    string waveformGenerator = _config["AudioProcessing:AudioWaveformPath"];
    if (waveformGenerator is null ||
        string.IsNullOrEmpty(waveformGenerator) ||
        !File.Exists(waveformGenerator)) {
      _logger.LogError( 
        "Audio waveform generator could not be found for {WaveformGenerator}",
        waveformGenerator);
    }

    var command = Cli.Wrap(waveformGenerator);
    _logger.LogInformation("Command is {Command}", command.ToString());
    var datResult = await command
      .WithArguments($"-i {file} -o {tmpDatFile} -b 8")
      .ExecuteBufferedAsync();
    _logger.LogInformation("DAT result is {DatResultStandardOutput}", datResult.StandardOutput);

    var jsonArgs = $"-i {file} -o {tmpJsonFile} --pixels-per-second 3 -b 8";
    _logger.LogInformation("JSON args {JsonArgs}", jsonArgs);
    var jsonResult = await command
      .WithArguments(jsonArgs)
      .ExecuteBufferedAsync();
    _logger.LogInformation("JSON result is {JsonResultStandardOutput}", jsonResult.StandardOutput);

    try {
      var pngResult = await command
        .WithArguments(
          $"-i {file} -o {tmpPngFile} -b 8 --no-axis-labels --colors audition --waveform-color baacf1FF --background-color 00000000")
        .ExecuteBufferedAsync();
      _logger.LogInformation("PNG result is {JsonResultStandardOutput}", jsonResult.StandardOutput);
      _logger.LogInformation("PNG error is {JsonResultStandardError}", jsonResult.StandardError);
    } catch (Exception e) {
      _logger.LogDebug("{Message}", e.Message);
    }


    File.Move(tmpDatFile, outputDatFile);
    File.Move(tmpJsonFile, outputJsonFile);
    File.Move(tmpPngFile, outputPngFile);
    return (
      File.Exists(outputDatFile) ? tmpDatFile : string.Empty,
      File.Exists(outputJsonFile) ? tmpJsonFile : string.Empty,
      File.Exists(outputPngFile) ? tmpPngFile : string.Empty
    );
  }
}
