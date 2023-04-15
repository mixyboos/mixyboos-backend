using System;
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

    public string GetLargeImageUrl(string imageType, string imageUrl) =>
        Flurl.Url.Combine(
            _config["Servers:ImageServer"],
            $"/img/{imageType}/{imageUrl}?width=1024&height=768");

    public string GetSmallImageUrl(string imageType, string imageUrl) =>
        Flurl.Url.Combine(
            _config["Servers:ImageServer"],
            $"/img/{imageType}/{imageUrl}?width=128&height=128");

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
