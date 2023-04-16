using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MixyBoos.Api.Data.Models;

[Index(nameof(Tag.TagName), IsUnique = true)]
public class Tag : BaseEntity {
    public string TagName { get; set; }
}
