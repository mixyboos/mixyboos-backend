using System.IO;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Extensions.Configuration;
using MixyBoos.Api.Data.Models;
using MixyBoos.Api.Services.Helpers;

namespace MixyBoos.Api.Data.Utils;

public class ImageCacher {
    private readonly ImageHelper _imageHelper;

    public ImageCacher(ImageHelper imageHelper) {
        _imageHelper = imageHelper;
    }

    public async Task CacheUserImages(MixyBoosUser user, IConfiguration config) {
        var faker = new Faker("en");
        user.ProfileImage = await _imageHelper.CacheImage(
            faker.Internet.Avatar(),
            Path.Combine(
                config["ImageProcessing:ImageRootFolder"],
                "users",
                "avatars",
                $"{user.Id}.jpg"));
        user.HeaderImage = await _imageHelper.CacheImage(
            faker.Internet.Avatar(),
            Path.Combine(
                config["ImageProcessing:ImageRootFolder"],
                "users",
                "headers",
                $"{user.Id}.jpg"));
    }
}
