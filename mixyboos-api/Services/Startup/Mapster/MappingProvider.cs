using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MixyBoos.Api.Data.DTO;
using MixyBoos.Api.Data.Models;
using MixyBoos.Api.Services.Extensions;
using MixyBoos.Api.Services.Helpers;

namespace MixyBoos.Api.Services.Startup.Mapster;

public static class MappingProvider {
  private static List<FollowDTO> _runMap(ICollection<MixyBoosUser> src) {
    return src.Select(s => new FollowDTO {
      Id = s.Id.ToString(),
      Name = s.DisplayName
    }).ToList();
  }

  private static bool _isLiked(Mix src, ClaimsPrincipal currentUser) {
    var userId = currentUser?.FindFirstValue(ClaimTypes.NameIdentifier);
    if (userId is null) {
      return false;
    }

    return src.Likes != null && src.Likes.Any(l => l.UserId.Equals(Guid.Parse(userId)));
  }

  public static void RegisterMapsterConfiguration(this IServiceCollection services, IConfiguration config,
    IServiceProvider serviceProvider) {
    var imageHelper = serviceProvider.GetRequiredService<ImageHelper>();
    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

    TypeAdapterConfig<Mix, MixDTO>
      .NewConfig()
      // .BeforeMapping((src, dest, context) => {
      //   var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
      //   var currentUser = httpContextAccessor.HttpContext?.User;
      // })
      .Map(dest => dest.Id, src => src.Id.ToString())
      .Map(dest => dest.Slug, src => src.Slug)
      .Map(dest => dest.DateUploaded, src => src.DateCreated)
      .Map(dest => dest.Image,
        src => imageHelper.GetLargeImageUrl("mixes", src.Image))
      .Map(dest => dest.Tags, src => src.Tags.Select(r => r))
      .Map(dest => dest.PlayCount, src => src.Plays.Count)
      .Map(dest => dest.LikeCount, src => src.Likes.Count)
      .Map(dest => dest.ShareCount, src => src.Shares.Count)
      .Map(dest => dest.DownloadCount, src => src.Downloads.Count)
      .Map(dest => dest.Duration, src => src.Duration.TotalSeconds)
      .Map(dest => dest.AudioUrl,
        src => Flurl.Url.Combine(config["LiveServices:ListenUrl"], src.Id.ToString(), $"{src.Id}.m3u8"))
      .Map(dest => dest.PcmUrl,
        src => Flurl.Url.Combine(config["LiveServices:PcmUrl"], src.Id.ToString(), $"{src.Id}.json"))
      .Map(dest => dest.IsLiked, (src) => _isLiked(src, httpContextAccessor.HttpContext.User));

    TypeAdapterConfig<MixDTO, Mix>
      .NewConfig()
      .Map(dest => dest.Duration, src => TimeSpan.FromSeconds(src.Duration))
      .Map(dest => dest.Id, src => src.Id);

    TypeAdapterConfig<LiveShow, LiveShowDTO>
      .NewConfig()
      .Map(dest => dest.Id, src => src.Id.ToString())
      .Map(dest => dest.Status, src => src.Status.ToString().ToCamelCase())
      .Map(dest => dest.Tags, src => src.Tags == null ? Array.Empty<string>() : src.Tags.Select(r => r.TagName));

    TypeAdapterConfig<LiveShow, CreateLiveShowDTO>
      .NewConfig()
      .Map(dest => dest.Tags, src => src.Tags.Select(r => r.TagName));

    TypeAdapterConfig<ShowChat, ShowChatDTO>
      .NewConfig()
      .Map(dest => dest.TimeStamp, src => src.DateCreated);

    TypeAdapterConfig<ProfileDTO, MixyBoosUser>
      .NewConfig()
      .Map(src => src.PhoneNumber, dest => dest.PhoneNumber);

    TypeAdapterConfig<MixyBoosUser, ProfileDTO>
      .NewConfig()
      .IgnoreNullValues(true);
  }
}
