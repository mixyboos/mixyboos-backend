﻿using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MixyBoos.Api.Services.Helpers;

public class ImageHelper {
  private readonly IConfiguration _config;

  public ImageHelper(IConfiguration config) {
    _config = config;
  }

  public string GetImage(string imageType, string profileImage) {
    return string.IsNullOrEmpty(profileImage) ? string.Empty :
      profileImage.StartsWith("http") ? profileImage :
      GetLargeImageUrl(imageType, profileImage);
  }

  //TODO: the Guid on the querystring is really yuck
  //TODO: find a better way to invalidate the cached image
  public string GetLargeImageUrl(string imageType, string imageUrl) =>
    Flurl.Url.Combine(
      _config["Servers:ImageServer"],
      $"/img/{imageType}/{imageUrl}?width=1024&height=768&t={Guid.NewGuid().ToString()}");

  public string GetSmallImageUrl(string imageType, string imageUrl) =>
    Flurl.Url.Combine(
      _config["Servers:ImageServer"],
      $"/img/{imageType}/{imageUrl}?width=128&height=128&t={Guid.NewGuid().ToString()}");

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
