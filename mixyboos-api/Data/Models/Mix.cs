#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Data.Utils;

namespace MixyBoos.Api.Data.Models;

[Index(nameof(Slug), IsUnique = true)]
public class Mix : BaseEntity, ISluggedEntity {
  [Required] public string? Title { get; set; }
  [Required] public string? Description { get; set; }
  public string? Image { get; set; }
  public string? AudioUrl { get; set; }
  public bool IsProcessed { get; set; } = false;
  
  public TimeSpan Duration { get; set; }
  [Required] public virtual MixyBoosUser? User { get; set; }

  public ICollection<MixPlay>? Plays { get; set; } = new List<MixPlay>();
  public ICollection<MixLike>? Likes { get; set; } = new List<MixLike>();
  public ICollection<MixShare>? Shares { get; set; } = new List<MixShare>();
  public ICollection<MixDownload>? Downloads { get; set; } = new List<MixDownload>();

  public ICollection<Tag> Tags { get; } = new List<Tag>();

  [SlugField(SourceField = "Title")] public string? Slug { get; set; }
}
