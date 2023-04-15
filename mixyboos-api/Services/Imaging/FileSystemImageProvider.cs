using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using MixyBoos.Api.Services.Extensions;
using SixLabors.ImageSharp.Web;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;

namespace MixyBoos.Api.Services.Imaging;

public class FileSystemImageProvider : IImageProvider {
    private readonly IConfiguration _config;
    private readonly IFileProvider _fileProvider;
    private readonly FormatUtilities _formatUtilities;

    private static readonly string[] _pathPrefixes = new[]
        {"/img"};

    public FileSystemImageProvider(IConfiguration config, IFileProvider fileProvider, FormatUtilities formatUtilities) {
        _config = config;
        _fileProvider = fileProvider;
        _formatUtilities = formatUtilities;
    }

    public bool IsValidRequest(HttpContext context) =>
        this._formatUtilities.TryGetExtensionFromUri(context.Request.GetDisplayUrl(), out _);

    private Func<HttpContext, bool> _match;

    public Task<IImageResolver> GetAsync(HttpContext context) {
        var filePath = Path.Combine(
            _config["ImageProcessing:ImageRootFolder"],
            _pathPrefixes.Select(r => context.Request.Path.Value.TrimStartString(r))
                .Aggregate((a, b) => $"{a}{b}")
        );
        var info = _fileProvider.GetFileInfo(filePath);

        // Check to see if the file exists.
        if (!info.Exists) {
            return Task.FromResult<IImageResolver>(null);
        }

        // We don't care about the content type nor cache control max age here.
        return Task.FromResult<IImageResolver>(new ImageResolver(info));
    }

    public ProcessingBehavior ProcessingBehavior { get; }

    public Func<HttpContext, bool> Match {
        get => _match ?? IsMatch;
        set => _match = value;
    }

    private bool IsMatch(HttpContext context) =>
        _pathPrefixes.Any(r => context.Request.Path.StartsWithSegments(r));
}

public class ImageResolver : IImageResolver {
    private readonly IFileInfo fileInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileProviderImageResolver"/> class.
    /// </summary>
    /// <param name="fileInfo">The file info.</param>
    public ImageResolver(IFileInfo fileInfo) => this.fileInfo = fileInfo;

    /// <inheritdoc/>
    public Task<ImageMetadata> GetMetaDataAsync() {
        return Task.FromResult(
            new ImageMetadata(
                this.fileInfo.LastModified.UtcDateTime,
                this.fileInfo.Length));
    }

    /// <inheritdoc/>
    public Task<Stream> OpenReadAsync() {
        return Task.FromResult(this.fileInfo.CreateReadStream());
    }
}

public class Metadata {
    public DateTime CreatedOn { get; internal set; }
}
