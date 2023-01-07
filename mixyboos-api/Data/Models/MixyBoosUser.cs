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

    [MaxLength(30)] public string DisplayName { get; set; }
    public string Image { get; set; }

    [SlugField(SourceField = "DisplayName")]
    public string Slug { get; set; }

    public string StreamKey { get; set; }

    public ICollection<MixyBoosUser> Followers { get; set; } 
    public ICollection<MixyBoosUser> Following { get; set; }
}
