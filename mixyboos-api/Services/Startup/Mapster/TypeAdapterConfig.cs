using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using MixyBoos.Api.Data.DTO;
using MixyBoos.Api.Data.Models;
using Mapster;

namespace MixyBoos.Api.Services.Startup.Mapster;

public static class TypeAdapterConfig {
    public static void RegisterMapsterConfiguration(this IServiceCollection services) {
        TypeAdapterConfig<Mix, MixDTO>
            .NewConfig()
            .Map(dest => dest.AudioUrl, src => $"http://localhost:8080/hls/{src.Id}/pl.m3u8")
            .Map(dest => dest.Image,
                src => string.IsNullOrEmpty(src.Image) ? "https://source.unsplash.com/random/800x600" : src.Image);

        TypeAdapterConfig<LiveShow, LiveShowDTO>
            .NewConfig()
            .Map(dest => dest.Tags, src => src.Tags.Select(r => r.TagName));
        
        TypeAdapterConfig<LiveShow, CreateLiveShowDTO>
            .NewConfig()
            .Map(dest => dest.Tags, src => src.Tags.Select(r => r.TagName));
        
    }
}
