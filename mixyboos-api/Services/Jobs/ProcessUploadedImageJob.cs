using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Data;
using Quartz;

namespace MixyBoos.Api.Services.Jobs;

public class ProcessUploadedImageJob : IJob {
    private readonly IConfiguration _config;
    private readonly MixyBoosContext _context;
    private readonly ILogger<ProcessUploadedImageJob> _logger;

    public ProcessUploadedImageJob(IConfiguration config,
        MixyBoosContext context,
        ILogger<ProcessUploadedImageJob> logger) {
        _config = config;
        _context = context;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context) {
        try {
            var data = context.Trigger.JobDataMap;
            var mixId = data["Id"]?.ToString();
            var imageSource = data["ImageSource"]?.ToString();
            var imageType = data["ImageType"]?.ToString();
            var fileLocation = data["FileLocation"]?.ToString();
            var outputPath = _config[$"ImageProcessing:{imageSource}Dir"];
            if (string.IsNullOrEmpty(outputPath)) {
                _logger.LogError("Unable to create output path for {FileLocation}", fileLocation);
                return;
            }

            _logger.LogInformation("Caching image for {Id} from {FileLocation} to {OutputPath}",
                mixId, fileLocation, outputPath);
            if (!Directory.Exists(outputPath)) {
                Directory.CreateDirectory(outputPath);
            }

            var destinationFile = Path.Combine(outputPath, imageType, Path.GetFileName(fileLocation));
            if (File.Exists(fileLocation) && Directory.Exists(outputPath)) {
                File.Move(fileLocation, destinationFile);
            }

            _logger.LogInformation("Successfully moved {Source} to {Destination}",
                fileLocation, destinationFile);

            _logger.LogInformation("Updating mix record");
            var mix = await _context
                .Mixes
                .AsTracking()
                .FirstOrDefaultAsync(m => m.Id.Equals(Guid.Parse(mixId)));
            mix.Image = Path.GetFileName(fileLocation);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully Updated mix record");
        } catch (Exception e) {
            _logger.LogError("Error caching image file\n\t{Error}", e.Message);
            throw;
        }
    }
}
