using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MixyBoos.Api.Data.Models;
using MixyBoos.Api.Data.Options;

namespace MixyBoos.Api.Services.Extensions;

public static class ModelBuilderExtensions {
  public static IEnumerable<PropertyBuilder> GetColumn(this ModelBuilder modelBuilder, string columnName) {
    return modelBuilder.Model
      .GetEntityTypes()
      .Where(t => t.Name.StartsWith("MixyBoos.Api.Data")) //this dude is all on her own!
      .SelectMany(t => t.GetProperties())
      .Where(p => p.Name == columnName)
      .Select(p => modelBuilder.Entity(p.DeclaringEntityType.ClrType).Property(p.Name));
  }

  public static void SeedAuthenticationUsers(this ModelBuilder builder, DbScaffoldOptions settings) {
    var passwordHasher = new PasswordHasher<MixyBoosUser>();
    var superAdminRole = new IdentityRole("SuperAdmin");
    superAdminRole.NormalizedName = superAdminRole.Name.ToUpper();
    var adminRole = new IdentityRole("Admin");
    adminRole.NormalizedName = adminRole.Name.ToUpper();
    var artistRole = new IdentityRole("Artist");
    artistRole.NormalizedName = artistRole.Name.ToUpper();
    var memberRole = new IdentityRole("Member");
    memberRole.NormalizedName = memberRole.Name.ToUpper();

    var roles = new List<IdentityRole> {
      superAdminRole,
      adminRole,
      artistRole,
      memberRole
    };

    builder.Entity<IdentityRole>().HasData(roles);

    var adminUser = new MixyBoosUser {
      Id = System.Guid.NewGuid(),
      Email = settings.AdminUserEmail,
      UserName = settings.AdminUserName,
      DisplayName = settings.AdminUserDisplayName,
      StreamKey = settings.AdminUserStreamKey,
      EmailConfirmed = true
    };
    adminUser.NormalizedUserName = adminUser.UserName.ToUpper();
    adminUser.NormalizedEmail = adminUser.Email.ToUpper();
    adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, settings.AdminUserPassword);

    var users = new List<MixyBoosUser> {
      adminUser
    };
    builder.Entity<MixyBoosUser>().HasData(users);
  }
}
