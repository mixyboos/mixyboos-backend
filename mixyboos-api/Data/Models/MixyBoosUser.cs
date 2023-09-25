using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Data.Utils;

namespace MixyBoos.Api.Data.Models;

[Index(nameof(Slug), IsUnique = true)]
public class MixyBoosUser : IdentityUser<Guid>, ISluggedEntity {
    public MixyBoosUser() {
        this.Followers = new List<MixyBoosUser>();
        this.Following = new List<MixyBoosUser>();
        this.Likes = new List<MixLike>();
        this.Plays = new List<MixPlay>();
        this.Shares = new List<MixShare>();
        this.Downloads = new List<MixDownload>();
    }

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

    public ICollection<MixyBoosUser> Followers { get; set; }
    public ICollection<MixyBoosUser> Following { get; set; }

    public ICollection<MixPlay> Plays { get; set; }
    public ICollection<MixLike>? Likes { get; set; }
    public ICollection<MixShare>? Shares { get; set; }
    public ICollection<MixDownload>? Downloads { get; set; }
}
