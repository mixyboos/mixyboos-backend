using System;
using System.IO;
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

  private async Task _updateUserImageDetails(string id, string imageType, string path) {
    _logger.LogInformation("Updating user record");
    var user = await _context
      .Users
      .FirstOrDefaultAsync(m => m.Id.Equals(Guid.Parse(id)));
    if (user is null) {
      _logger.LogError("Unable to fond user in db {MixId}", id);
      return;
    }

    if (imageType.Equals("headers")) {
      user.HeaderImage = Path.GetFileName(path);
    }

    if (imageType.Equals("avatars")) {
      user.ProfileImage = Path.GetFileName(path);
    }

    await _context.SaveChangesAsync();
  }

  private async Task _updateMixImageDetails(string id, string path) {
    _logger.LogInformation("Updating mix record");
    var mix = await _context
      .Mixes
      .FirstOrDefaultAsync(m => m.Id.Equals(Guid.Parse(id)));
    if (mix is null) {
      _logger.LogError("Unable to fond mix in db {MixId}", id);
      return;
    }

    mix.Image = Path.GetFileName(path);
    await _context.SaveChangesAsync();
  }

  public async Task Execute(IJobExecutionContext context) {
    try {
      var data = context.Trigger.JobDataMap;
      var id = data["Id"]?.ToString();
      var imageSource = data["ImageSource"]?.ToString();
      var imageType = data["ImageType"]?.ToString();
      var fileLocation = data["FileLocation"]?.ToString();
      var outputPath = _config[$"ImageProcessing:{imageSource}Dir"];
      if (string.IsNullOrEmpty(outputPath)) {
        _logger.LogError("Unable to create output path for {FileLocation}", fileLocation);
        return;
      }

      _logger.LogInformation("Caching image for {Id} from {FileLocation} to {OutputPath}",
        id, fileLocation, outputPath);
      if (!Directory.Exists(outputPath)) {
        Directory.CreateDirectory(outputPath);
      }

      var destinationFile = Path.Combine(outputPath, imageType ?? string.Empty, Path.GetFileName(fileLocation));
      if (File.Exists(fileLocation) && Directory.Exists(outputPath)) {
        if (File.Exists(destinationFile)) {
          File.Delete(destinationFile);
        }

        File.Move(fileLocation, destinationFile);
      }

      _logger.LogInformation("Successfully moved {Source} to {Destination}",
        fileLocation, destinationFile);

      switch (imageSource) {
        case "MixImage":
          await _updateMixImageDetails(id, destinationFile);
          break;
        case "UserImage":
          await _updateUserImageDetails(id, imageType, destinationFile);
          break;
      }
    } catch (Exception e) {
      _logger.LogError("Error caching image file\n\t{Error}", e.Message);
      throw;
    }
  }
}
