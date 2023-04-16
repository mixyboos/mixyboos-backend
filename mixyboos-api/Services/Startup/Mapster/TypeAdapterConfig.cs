using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using Microsoft.Extensions.DependencyInjection;
using MixyBoos.Api.Data.DTO;
using MixyBoos.Api.Data.Models;
using Mapster;
using Microsoft.Extensions.Configuration;
using MixyBoos.Api.Data;
using MixyBoos.Api.Services.Helpers;

namespace MixyBoos.Api.Services.Startup.Mapster;

public static class TypeAdapterConfig {
    private static List<FollowDTO> _runMap(ICollection<MixyBoosUser> src) {
        return src.Select(s => new FollowDTO {
            Id = s.Id, Name = s.DisplayName
        }).ToList();
    }

    public static void RegisterMapsterConfiguration(this IServiceCollection services, IConfiguration config) {
        var imageHelper = services.BuildServiceProvider().GetService<ImageHelper>();
        TypeAdapterConfig<Mix, MixDTO>
            .NewConfig()
            .Map(dest => dest.DateUploaded, src => src.DateCreated)
            .Map(dest => dest.Image,
                src => src.Image.StartsWith("http")
                    ? src.Image
                    : imageHelper.GetLargeImageUrl("mixes", src.Image))
            .Map(dest => dest.PlayCount, src => src.Plays != null ? src.Plays.Count : 0);

        TypeAdapterConfig<LiveShow, LiveShowDTO>
            .NewConfig()
            .Map(dest => dest.Tags, src => src.Tags == null ? Array.Empty<string>() : src.Tags.Select(r => r.TagName));

        TypeAdapterConfig<LiveShow, CreateLiveShowDTO>
            .NewConfig()
            .Map(dest => dest.Tags, src => src.Tags.Select(r => r.TagName));

        TypeAdapterConfig<ProfileDTO, MixyBoosUser>
            .NewConfig()
            .Ignore(src => src.Id)
            .Map(src => src.PhoneNumber, dest => dest.PhoneNumber);

        TypeAdapterConfig<MixyBoosUser, ProfileDTO>
            .NewConfig()
            .Map(dest => dest.HeaderImage,
                src => src.HeaderImage.StartsWith("http")
                    ? src.HeaderImage
                    : imageHelper.GetLargeImageUrl("users/headers", src.HeaderImage))
            .Map(dest => dest.ProfileImage,
                src => src.ProfileImage.StartsWith("http")
                    ? src.ProfileImage
                    : imageHelper.GetSmallImageUrl("users/avatars", src.ProfileImage))
            .Map(dest => dest.Followers, src => _runMap(src.Followers))
            .Map(dest => dest.Following, src => _runMap(src.Following));
    }
}
