using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Data.Utils;

namespace MixyBoos.Api.Data.Models;

[Index(nameof(Name), IsUnique = true)]
[Index(nameof(Slug), IsUnique = true)]
public class Tag : BaseEntity, ISluggedEntity {
  [Required]
  [MaxLength(50)]
  public string Name { get; set; }

  [MaxLength(30)]
  [SlugField(SourceField = "Name")]
  public string Slug { get; set; }
}
