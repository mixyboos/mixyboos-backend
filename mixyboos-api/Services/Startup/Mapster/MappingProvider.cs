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
using MixyBoos.Api.Data.Utils;
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
      .Map(dest => dest.Id, src => src.Id.ToString())
      .Map(dest => dest.Slug, src => src.Slug)
      .Map(dest => dest.DateUploaded, src => src.DateCreated)
      .Map(dest => dest.Image,
        src => imageHelper.GetLargeImageUrl("mixes", src.Image))
      .Map(dest => dest.Tags, src => src.Tags.Select(r => r.Name).ToArray())
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
      .Map(dest => dest.Id, src => src.Id)
      .Ignore(dest => dest.Tags)
      .AfterMappingAsync(async (src, dest) => {
        // Skip if Tags is null (not provided), but process empty array to clear all tags
        if (src.Tags == null) return;

        // If empty array, clear all tags
        if (src.Tags.Length == 0) {
          dest.Tags.Clear();
          return;
        }

        var tagConverter = httpContextAccessor.HttpContext?.RequestServices.GetService<TagConverter>();
        if (tagConverter == null) return;

        var tagNames = src.Tags.ToList();
        var incomingTags = await tagConverter.ProcessTagsPayload(tagNames);

        var existingTagIds = dest.Tags.Select(t => t.Id).ToHashSet();
        var incomingTagIds = incomingTags.Select(t => t.Id).ToHashSet();

        // Remove tags no longer present
        var tagsToRemove = dest.Tags.Where(t => !incomingTagIds.Contains(t.Id)).ToList();
        foreach (var tag in tagsToRemove) {
          dest.Tags.Remove(tag);
        }

        // Add new tags not already present
        foreach (var tag in incomingTags.Where(t => !existingTagIds.Contains(t.Id))) {
          dest.Tags.Add(tag);
        }
      });

    TypeAdapterConfig<LiveShow, LiveShowDTO>
      .NewConfig()
      .Map(dest => dest.Id, src => src.Id.ToString())
      .Map(dest => dest.Status, src => src.Status.ToString().ToCamelCase())
      .Map(dest => dest.Tags, src => src.Tags == null ? new List<string>() : src.Tags.Select(r => r.Name).ToList());

    TypeAdapterConfig<CreateLiveShowDTO, LiveShow>
      .NewConfig()
      .AfterMappingAsync(async (src, dest) => {
        if (src.Tags != null && src.Tags.Count > 0) {
          var tagConverter = httpContextAccessor.HttpContext?.RequestServices.GetService<TagConverter>();
          if (tagConverter != null) {
            var tags = await tagConverter.ProcessTagsPayload(src.Tags);
            dest.Tags.Clear();
            foreach (var tag in tags) {
              dest.Tags.Add(tag);
            }
          }
        }
      });

    TypeAdapterConfig<ShowChat, ShowChatDTO>
      .NewConfig()
      .Map(dest => dest.TimeStamp, src => src.DateCreated);

    TypeAdapterConfig<ProfileDTO, MixyBoosUser>
      .NewConfig()
      .Map(src => src.PhoneNumber, dest => dest.PhoneNumber);

    TypeAdapterConfig<MixyBoosUser, ProfileDTO>
      .NewConfig()
      .IgnoreNullValues(true);


    TypeAdapterConfig<MixyBoosUser, UserDTO>
      .NewConfig()
      .IgnoreNullValues(true)
      .Map(src => src.ProfileImage, src => imageHelper.GetLargeImageUrl("users/avatars", src.ProfileImage))
      .Map(src => src.HeaderImage, src => imageHelper.GetLargeImageUrl("users/headers", src.HeaderImage));
  }
}
