using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace MixyBoos.Api.Services.Jobs;

public class ProcessUploadedImageJob : IJob {
    private readonly IConfiguration _config;
    private readonly ILogger<ProcessUploadedImageJob> _logger;

    public ProcessUploadedImageJob(IConfiguration config,
        ILogger<ProcessUploadedImageJob> logger) {
        _config = config;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context) {
        await Task.Factory.StartNew(() => {
            var data = context.Trigger.JobDataMap;
            var imageType = data["ImageType"]?.ToString();
            var fileLocation = data["FileLocation"]?.ToString();
            var outputPath = _config[$"ImageProcessing:{imageType}Dir"];
            if (string.IsNullOrEmpty(outputPath)) {
                _logger.LogError("Unable to create output path for {FileLocation}", fileLocation);
                return;
            }

            if (!Directory.Exists(outputPath)) {
                Directory.CreateDirectory(outputPath);
            }

            if (File.Exists(fileLocation) && Directory.Exists(outputPath)) {
                File.Move(fileLocation, Path.Combine(outputPath, Path.GetFileName(fileLocation)));
            }
        });
    }
}
