using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.Models;

namespace MixyBoos.Api.Services.Extensions;

public static class DbContextExtensions {
    public static async Task<ICollection<Tag>> MapTags(this MixyBoosContext context, List<string> tags) {
        ICollection<Tag> result = new List<Tag>();
        if (tags is null)
            return result;

        foreach (var t in tags) {
            var tag = await context.Tags.Where(r => r.TagName.Equals(t)).FirstOrDefaultAsync() ?? new Tag {
                TagName = t
            };
            result.Add(tag);
        }

        return result;
    }
}
