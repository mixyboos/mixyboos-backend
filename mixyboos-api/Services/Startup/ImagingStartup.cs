using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MixyBoos.Api.Services.Imaging;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Processors;
using SixLabors.ImageSharp.Web.Providers;

namespace MixyBoos.Api.Services.Startup;

public static class ImagingStartup {
  public static IServiceCollection AddImaging(this IServiceCollection services, IConfiguration config) {
    services.AddImageSharp()
      .Configure<PhysicalFileSystemCacheOptions>(options => {
          options.CacheFolder = config["ImageProcessing:ImageCacheFolder"] ?? "mb-cache";
        }
      )
      .SetRequestParser<QueryCollectionRequestParser>()
      .ClearProviders()
      .AddProvider<FileSystemImageProvider>()
      .AddProcessor<ResizeWebProcessor>();
    return services;
  }
}
