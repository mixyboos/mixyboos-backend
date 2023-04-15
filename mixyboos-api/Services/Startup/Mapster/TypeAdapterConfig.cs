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

namespace MixyBoos.Api.Services.Startup.Mapster;

public static class TypeAdapterConfig {
    private static string _runImageMap(string src) {
        var faker = new Faker();
        return string.IsNullOrEmpty(src) ? faker.Image.LoremFlickrUrl() : src;
    }

    private static List<FollowDTO> _runMap(ICollection<MixyBoosUser> src) {
        return src.Select(s => new FollowDTO {
            Id = s.Id, Name = s.DisplayName
        }).ToList();
    }

    public static void RegisterMapsterConfiguration(this IServiceCollection services, IConfiguration config) {
        TypeAdapterConfig<Mix, MixDTO>
            .NewConfig()
            .Map(dest => dest.DateUploaded, src => src.DateCreated)
            .Map(dest => dest.Image, src => _runImageMap(src.Image))
            .Map(dest => dest.PlayCount, src => src.Plays != null ? src.Plays.Count : 0);

        TypeAdapterConfig<LiveShow, LiveShowDTO>
            .NewConfig()
            .Map(dest => dest.Tags, src => src.Tags == null ? Array.Empty<string>() : src.Tags.Select(r => r.TagName));

        TypeAdapterConfig<LiveShow, CreateLiveShowDTO>
            .NewConfig()
            .Map(dest => dest.Tags, src => src.Tags.Select(r => r.TagName));

        TypeAdapterConfig<MixyBoosUser, ProfileDTO>
            .NewConfig()
            .Map(dest => dest.Followers, src => _runMap(src.Followers))
            .Map(dest => dest.Following, src => _runMap(src.Following));
    }
}
