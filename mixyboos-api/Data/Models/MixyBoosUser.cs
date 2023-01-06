using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Data.Utils;

namespace MixyBoos.Api.Data; 

[Index(nameof(Slug), IsUnique = true)]
public class MixyBoosUser : IdentityUser, ISluggedEntity {
    public string DisplayName { get; set; }
    public string Image { get; set; }

    [SlugField(SourceField = "DisplayName")]
    public string Slug { get; set; }

    public string StreamKey { get; set; }
}
