using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MixyBoos.Api.Data.Seeders;

namespace MixyBoos.Api.Services.Extensions {
    public static class ModelBuilderExtensions {
        public static IEnumerable<PropertyBuilder> GetColumn(this ModelBuilder modelBuilder, string columnName) {
            return modelBuilder.Model
                .GetEntityTypes()
                .Where(t => t.Name.StartsWith("MixyBoos.Api.Data")) //this dude is all on her own!
                .SelectMany(t => t.GetProperties())
                .Where(p => p.Name == columnName)
                .Select(p => modelBuilder.Entity(p.DeclaringEntityType.ClrType).Property(p.Name));
        }
    }
}
