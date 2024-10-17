using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Data.DTO;
using MixyBoos.Api.Data.Utils;

namespace MixyBoos.Api.Data.Models;

[Index(nameof(Slug), IsUnique = true)]
public class MixyBoosUser : IdentityUser<Guid>, ISluggedEntity {
  [MaxLength(50)] public string Title { get; set; }
  [MaxLength(30)] public string DisplayName { get; set; }
  public string ProfileImage { get; set; }
  public string HeaderImage { get; set; }

  [SlugField(SourceField = "UserName")]
  public string Slug { get; set; }

  [MaxLength(100)] public string City { get; set; }
  [MaxLength(100)] public string Country { get; set; }
  [MaxLength(2048)] public string Biography { get; set; }
  public string StreamKey { get; set; }

  public ICollection<MixyBoosUser> Followers { get; set; } = new List<MixyBoosUser>();
  public ICollection<MixyBoosUser> Following { get; set; } = new List<MixyBoosUser>();

  public ICollection<MixPlay> Plays { get; set; } = new List<MixPlay>();
  public ICollection<MixLike> Likes { get; set; } = new List<MixLike>();
  public ICollection<MixShare> Shares { get; set; } = new List<MixShare>();
  public ICollection<MixDownload> Downloads { get; set; } = new List<MixDownload>();


  public void FromDto(ProfileDTO profile) {
    Slug = profile.Slug;
    Title = profile.Title;
    DisplayName = profile.DisplayName;
    City = profile.City;
    Country = profile.Country;
    Biography = profile.Biography;
    PhoneNumber = profile.PhoneNumber;
  }

  public ProfileDTO ToDto() => this.Adapt<ProfileDTO>();
}
