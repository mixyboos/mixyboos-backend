using System;
using System.IO;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Buffered;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Services.Helpers.IO;

namespace MixyBoos.Api.Services.Helpers.Audio;

public class WaveformGenerationProgress {
  public int Percentage { get; set; }
  public string Status { get; set; }
}

public class WaveformGenerationResult {
  public string PNG { get; set; }
  public string JSON { get; set; }
  public string DAT { get; set; }
}

public interface IWaveformGenerator {
  Task<WaveformGenerationResult> GenerateWaveformFromFile(string file, string id,
    Action<string> waveformProgressHandler);
}

public class WaveformGenerator(ILogger<WaveformGenerator> logger, IConfiguration config) :
  IWaveformGenerator {
  public async Task<WaveformGenerationResult> GenerateWaveformFromFile(string file, string id,
    Action<string> waveformProgressHandler) {
    logger.LogInformation("Generating waveform for {LocalFile}", file);
    //TODO: Move config to POCO's to avoid "Possible null assignments"
    var outputDir = Path.Combine(config["AudioProcessing:WaveformDir"], id);
    if (string.IsNullOrEmpty(outputDir)) {
      logger.LogError("Output directory is missing");
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

    string waveformGenerator = config["AudioProcessing:AudioWaveformPath"];
    if (string.IsNullOrEmpty(waveformGenerator)) {
      logger.LogError(
        "Waveform generator is missing in config {WaveformGenerator}",
        waveformGenerator);
      throw new InvalidOperationException("Waveform generator is missing in config");
    }

    var command = Cli.Wrap(waveformGenerator);
    logger.LogInformation("Command is {Command}", command.ToString());
    var datResult = await command
      .WithArguments($"-i {file} -o {tmpDatFile} -b 8")
      .ExecuteBufferedAsync();
    logger.LogInformation("DAT result is {DatResultStandardOutput}", datResult.StandardOutput);

    var jsonArgs = $"-i {file} -o {tmpJsonFile} --pixels-per-second 3 -b 8";
    logger.LogInformation("JSON args {JsonArgs}", jsonArgs);
    var jsonResult = await command
      .WithArguments(jsonArgs)
      .ExecuteBufferedAsync();
    logger.LogInformation("JSON result is {JsonResultStandardOutput}", jsonResult.StandardOutput);

    try {
      var pngResult = await command
        .WithArguments(
          $"-i {file} -o {tmpPngFile} -b 8 --no-axis-labels --colors audition " +
          $"--waveform-color baacf1FF --background-color 00000000")
        .ExecuteBufferedAsync();
      logger.LogInformation("PNG result is {JsonResultStandardOutput}", jsonResult.StandardOutput);
      logger.LogInformation("PNG error is {JsonResultStandardError}", jsonResult.StandardError);
    } catch (Exception e) {
      logger.LogDebug("{Message}", e.Message);
    }


    File.Move(tmpDatFile, outputDatFile);
    File.Move(tmpJsonFile, outputJsonFile);
    File.Move(tmpPngFile, outputPngFile);
    return new WaveformGenerationResult {
      DAT = File.Exists(outputDatFile) ? tmpDatFile : string.Empty,
      JSON = File.Exists(outputJsonFile) ? tmpJsonFile : string.Empty,
      PNG = File.Exists(outputPngFile) ? tmpPngFile : string.Empty
    };
  }
}
