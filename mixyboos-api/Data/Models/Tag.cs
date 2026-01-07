using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MixyBoos.Api.Data.Models;

[Index(nameof(Name), IsUnique = true)]
[Index(nameof(Slug), IsUnique = true)]
public class Tag : BaseEntity {
  [MaxLength(50)]
  public string Name { get; set; }

  [MaxLength(30)]
  public string Slug { get; set; }
}
