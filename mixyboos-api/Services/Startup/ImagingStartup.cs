using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MixyBoos.Api.Services.Imaging;
using SixLabors.ImageSharp.Web;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Processors;
using SixLabors.ImageSharp.Web.Providers;

namespace MixyBoos.Api.Services.Startup;

public static class ImagingStartup {
    public static IServiceCollection AddImaging(this IServiceCollection services, IConfiguration config) {
        services.AddImageSharp()
            .SetRequestParser<QueryCollectionRequestParser>()
            .ClearProviders()
            .Configure<PhysicalFileSystemCacheOptions>(options => {
                options.CacheRootPath = ".img-cache";
            })
            .Configure<PhysicalFileSystemProviderOptions>(options => {
                options.ProviderRootPath = config["ImageProcessing:ImageCacheFolder"] ?? "/tmp";
                options.ProcessingBehavior = ProcessingBehavior.All;
            })
            .SetCache<PhysicalFileSystemCache>()
            .AddProvider<FileSystemImageProvider>()
            // .AddProvider(PhysicalFileSystemProviderFactory)
            .AddProcessor<ResizeWebProcessor>();
        return services;
    }
}
