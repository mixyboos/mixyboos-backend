using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Data.Models;

namespace MixyBoos.Api.Services.Extensions;

public static class AddOrUpdateExtensions {
    public static async Task<int> AddOrUpdate<T>(this DbContext context, T entity)
        where T : BaseEntity {
        return await context.AddOrUpdateRange(new[] {entity});
    }

    public static async Task<int> AddOrUpdateRange<T>(this DbContext context, IEnumerable<T> entities)
        where T : BaseEntity {
        foreach (var entity in entities) {
            var id = entity.Id;

            // null resolution operator casts to object, so use ternary
            var tracked = (id != Guid.Empty)
                ? await context.Set<T>().FindAsync(id)
                : await context.Set<T>().FindAsync(entity.Id);

            if (tracked != null) {
                // perform shallow copy
                context.Entry(tracked).CurrentValues.SetValues(entity);
            } else {
                context.Entry(entity).State = EntityState.Added;
            }
        }

        return await context.SaveChangesAsync();
    }
}
