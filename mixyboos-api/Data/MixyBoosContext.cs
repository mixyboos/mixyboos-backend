using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MixyBoos.Api.Data.Models;
using MixyBoos.Api.Data.Utils;
using MixyBoos.Api.Services.Extensions;
using OpenIddict.EntityFrameworkCore.Models;

#nullable disable

namespace MixyBoos.Api.Data;

public class MixyBoosContext : IdentityDbContext<MixyBoosUser> {
    private const string IDENTITY_PREFIX = "openiddict";
    public DbSet<Mix> Mixes { get; set; }
    public DbSet<MixPlay> MixPlays { get; set; }
    public DbSet<LiveShow> LiveShows { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ShowChat> ShowChat { get; set; }

    public MixyBoosContext() { }

    public MixyBoosContext(DbContextOptions<MixyBoosContext> options)
        : base(options) { }

    private IEnumerable<PropertyBuilder> __getColumns(ModelBuilder modelBuilder, string columnName) {
        //helper function to only return models which are part of this project
        //not models created by separate libs (auth, openiddict etc.)
        return modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(p => p.DeclaringEntityType.ClrType.IsSubclassOf(typeof(BaseEntity)))
            .Where(p => p.Name == columnName)
            .Select(p => modelBuilder.Entity(p.DeclaringEntityType.ClrType).Property(p.Name));
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (!optionsBuilder.IsConfigured) {
            optionsBuilder
                .UseNpgsql("Name=MixyBoos")
                .AddInterceptors(new MixyBoosDbEventsInterceptor())
                .UseSnakeCaseNamingConvention();
        }
    }

    protected override void OnModelCreating(ModelBuilder mb) {
        base.OnModelCreating(mb);

        mb.HasPostgresExtension("uuid-ossp");
        mb.HasAnnotation("Relational:Collation", "en_US.utf8");

        //give the identity tables proper names and schema
        mb.Entity<MixyBoosUser>().ToTable("user", "oid");
        mb.Entity<IdentityUser>().ToTable("identity_user", "oid");
        mb.Entity<IdentityRole>().ToTable("user_role", "oid");
        mb.Entity<IdentityUserClaim<string>>().ToTable("user_claim", "oid");
        mb.Entity<IdentityUserLogin<string>>().ToTable("user_login", "oid");
        mb.Entity<IdentityRoleClaim<string>>().ToTable("role_claim", "oid");
        mb.Entity<IdentityUserToken<string>>().ToTable("user_token", "oid");
        mb.Entity<IdentityUserRole<string>>().ToTable("user_identity_role", "oid");

        mb.Entity<OpenIddictEntityFrameworkCoreApplication>().ToTable($"{IDENTITY_PREFIX}_{"application"}", "oid");
        mb.Entity<OpenIddictEntityFrameworkCoreAuthorization>().ToTable($"{IDENTITY_PREFIX}_{"authorization"}", "oid");
        mb.Entity<OpenIddictEntityFrameworkCoreScope>().ToTable($"{IDENTITY_PREFIX}_{"scope"}", "oid");
        mb.Entity<OpenIddictEntityFrameworkCoreToken>().ToTable($"{IDENTITY_PREFIX}_{"token"}", "oid");
        //end identity stuff

        foreach (var pb in __getColumns(mb, "DateCreated")) {
            pb.ValueGeneratedOnAdd()
                .HasDefaultValueSql("now()");
        }

        foreach (var pb in __getColumns(mb, "DateUpdated")) {
            pb.ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("now()");
        }

        foreach (var pb in __getColumns(mb, "Id")) {
            pb.ValueGeneratedOnAdd()
                .HasColumnName("id")
                .HasColumnType("uuid")
                .HasDefaultValueSql("uuid_generate_v4()") // Use 
                .IsRequired();
        }

        mb.Entity<MixyBoosUser>()
            .HasMany(p => p.Following)
            .WithMany(p => p.Followers)
            .UsingEntity(j => j.ToTable("user_followers"));

        mb.Entity<Mix>()
            .HasMany(m => m.Plays)
            .WithOne(m => m.Mix);

        mb.Entity<Mix>()
            .Navigation(m => m.Plays)
            .AutoInclude();

        mb.Entity<Mix>()
            .Navigation(m => m.User)
            .AutoInclude();
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default) {
        foreach (var entity in ChangeTracker.Entries()
                     .Where(e => e.State is EntityState.Added or EntityState.Modified)
                     .Where(e => e.Entity is ISluggedEntity)
                     .Select(e => e.Entity as ISluggedEntity)
                     .Where(e => string.IsNullOrEmpty(e.Slug))) {
            entity.Slug = entity.GenerateSlug(this);
        }

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
