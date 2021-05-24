using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace MixyBoos.Api.Data {
    [Index(nameof(Slug), IsUnique = true)]
    public class Mix : BaseEntity, ISluggedEntity {
        [SlugField(SourceField = "Title")] public string Slug { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string AudioUrl { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
