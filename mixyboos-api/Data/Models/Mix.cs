using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Data.Utils;

#nullable enable

namespace MixyBoos.Api.Data.Models;

[Index(nameof(Slug), IsUnique = true)]
public class Mix : BaseEntity, ISluggedEntity {
    public Mix() {
        this.Tags = new List<Tag>();
        this.Likes = new List<MixLike>();
        this.Plays = new List<MixPlay>();
        this.Shares = new List<MixShare>();
        this.Downloads = new List<MixDownload>();
    }

    [SlugField(SourceField = "Title")] public string? Slug { get; set; }
    [Required] public string? Title { get; set; }
    [Required] public string? Description { get; set; }
    public string? Image { get; set; }
    public string? AudioUrl { get; set; }
    public bool IsProcessed { get; set; } = false;

    [Required] public virtual MixyBoosUser? User { get; set; }

    public ICollection<MixPlay>? Plays { get; set; }
    public ICollection<MixLike>? Likes { get; set; }
    public ICollection<MixShare>? Shares { get; set; }
    public ICollection<MixDownload>? Downloads { get; set; }

    public ICollection<Tag> Tags { get; }
}
