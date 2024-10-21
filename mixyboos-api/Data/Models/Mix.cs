#nullable enable

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Data.Utils;

namespace MixyBoos.Api.Data.Models;

[Index(nameof(Slug), IsUnique = true)]
public class Mix : BaseEntity, ISluggedEntity {
  public Mix() {
    Tags = new List<Tag>();
    Likes = new List<MixLike>();
    Plays = new List<MixPlay>();
    Shares = new List<MixShare>();
    Downloads = new List<MixDownload>();
  }

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

  [SlugField(SourceField = "Title")] public string? Slug { get; set; }
}
