using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CliWrap;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Services.Helpers;
using Quartz;

namespace MixyBoos.Api.Services.Jobs {
    public class ConvertAudioJob : IJob {
        private readonly ILogger<ConvertAudioJob> _logger;

        public ConvertAudioJob(ILogger<ConvertAudioJob> logger) {
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context) {
            var data = context.Trigger.JobDataMap;
            var id = data["Id"]?.ToString();

            if (string.IsNullOrEmpty(id)) {
                _logger.LogError("Error processing {Id} - invalid id", data["Id"]);
            }

            var outputDir = Path.Combine(Path.GetTempPath(), id!);

            Directory.CreateDirectory(outputDir);

            var argBuilder = new StringBuilder();
            argBuilder.Append($"-i {data["FileLocation"]} ");
            argBuilder.Append($"-c:a aac ");
            argBuilder.Append($"-b:a 320k ");
            argBuilder.Append($"-master_pl_name pl.m3u8 ");
            argBuilder.Append($"-f hls -hls_time 6 -hls_list_size 0 ");
            argBuilder.Append($"-hls_segment_filename \"{outputDir}/v%v/p_%d.m4a\" ");
            argBuilder.Append($"\"{outputDir}/v%v/i.m3u8\" ");

            if (context.CancellationToken.IsCancellationRequested) return;
            _logger.LogDebug("ffmpeg\n\t{Args}", argBuilder.ToString());

            var result = await Cli.Wrap("ffmpeg")
                .WithArguments(argBuilder.ToString())
                .WithWorkingDirectory(outputDir)
                .WithStandardOutputPipe(PipeTarget.ToDelegate(p => _logger.LogDebug("{Output}", p)))
                .ExecuteAsync(context.CancellationToken);

            if (result.ExitCode != 0) {
                _logger.LogError(
                    "Error processing {Id} - {ResultCode} - {Result}",
                    id,
                    result.ExitCode,
                    result.ToString());
                return;
            }

            FileHelpers.CopyFilesRecursively(
                outputDir,
                Path.Combine("/home/fergalm/working/mixyboos/audio/", id)
            );

            _logger.LogInformation("Finished processing {Id}", id);
        }
    }
}
