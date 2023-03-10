using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Data.Utils;

namespace MixyBoos.Api.Data;

[Index(nameof(Slug), IsUnique = true)]
public class MixyBoosUser : IdentityUser, ISluggedEntity {
    public MixyBoosUser() {
        Followers = new List<MixyBoosUser>();
        Following = new List<MixyBoosUser>();
    }

    [MaxLength(50)] public string Title { get; set; }
    [MaxLength(30)] public string DisplayName { get; set; }
    public string ProfileImage { get; set; }
    public string HeaderImage { get; set; }

    [SlugField(SourceField = "DisplayName")]
    public string Slug { get; set; }

    [MaxLength(1000)] public string Biography { get; set; }
    public string StreamKey { get; set; }

    public ICollection<MixyBoosUser> Followers { get; set; }
    public ICollection<MixyBoosUser> Following { get; set; }
}
