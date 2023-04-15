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
        var data = context.Trigger.JobDataMap;
        var mixId = data["Id"]?.ToString();
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

        var mix = await _context
            .Mixes
            .AsTracking()
            .FirstOrDefaultAsync(m => m.Id.Equals(Guid.Parse(mixId)));
        mix.Image = Path.GetFileName(fileLocation);

        await _context.SaveChangesAsync();
    }
}
