using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MixyBoos.Api.Services.Helpers;

public class ImageHelper {
  private readonly IConfiguration _config;
  private readonly ILogger<ImageHelper> _logger;

  public ImageHelper(IConfiguration config, ILogger<ImageHelper> logger) {
    _config = config;
    _logger = logger;
  }

  public string GetImage(string imageType, string profileImage) {
    return string.IsNullOrEmpty(profileImage) ? string.Empty :
      profileImage.StartsWith("http") ? profileImage :
      GetLargeImageUrl(imageType, profileImage);
  }

  //TODO: the Guid on the querystring is really yuck
  //TODO: find a better way to invalidate the cached image
  public string GetLargeImageUrl(string imageType, string imageUrl) {
    try {
      if (string.IsNullOrEmpty(imageUrl)) {
        return _config["SiteSettings:DefaultMixImage"];
      }

      if (imageUrl.StartsWith("http")) {
        return imageUrl;
      }

      var url = Url.Combine(
        _config["Servers:ImageServer"],
        $"/img/{imageType}/{imageUrl}?width=1024&height=768&t={Guid.NewGuid().ToString()}");
      return url;
    } catch (Exception e) {
      _logger.LogError("Unable to create image URL Type: {ImageType} URL: {ImageUrl} - {Message}",
        imageType, imageUrl, e.Message);
    }

    return _config["SiteSettings:DefaultMixImage"];
  }

  public string GetSmallImageUrl(string imageType, string imageUrl) {
    return Url.Combine(
      _config["Servers:ImageServer"],
      $"/img/{imageType}/{imageUrl}?width=128&height=128&t={Guid.NewGuid().ToString()}");
  }

  public async Task<string> CacheImage(string source, string destination) {
    try {
      if (string.IsNullOrEmpty(destination)) {
        return destination;
      }

      if (File.Exists(destination)) {
        File.Delete(destination);
      }

      using var httpClient = new HttpClient();

      var imageBytes = await httpClient.GetByteArrayAsync(source);
      await File.WriteAllBytesAsync(destination, imageBytes);
    } catch (Exception e) {
      Console.WriteLine($"Error caching image {e.Message}");
    }

    return Path.GetFileName(destination);
  }
}
