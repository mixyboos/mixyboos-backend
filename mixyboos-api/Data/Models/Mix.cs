using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Data.Utils;

#nullable disable

namespace MixyBoos.Api.Data.Models;

[Index(nameof(Slug), IsUnique = true)]
public class Mix : BaseEntity, ISluggedEntity {
    [SlugField(SourceField = "Title")] public string Slug { get; set; }
    [Required] public string Title { get; set; }
    [Required] public string Description { get; set; }
    public string Image { get; set; }
    public string AudioUrl { get; set; }
    public bool IsProcessed { get; set; } = false;
    [Required] public virtual MixyBoosUser User { get; set; }

    public ICollection<MixPlay> Plays { get; set; }
}
