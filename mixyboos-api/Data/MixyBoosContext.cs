using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MixyBoos.Api.Data.Models;
using MixyBoos.Api.Data.Utils;
using MixyBoos.Api.Services.Extensions;
using OpenIddict.EntityFrameworkCore.Models;

#nullable disable

namespace MixyBoos.Api.Data;

public class MixyBoosContext : IdentityDbContext<MixyBoosUser> {
    private const string IDENTITY_PREFIX = "openiddict";
    public DbSet<Mix> Mixes { get; set; }
    public DbSet<LiveShow> LiveShows { get; set; }
    public DbSet<ShowChat> ShowChat { get; set; }

    public MixyBoosContext() { }

    public MixyBoosContext(DbContextOptions<MixyBoosContext> options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (!optionsBuilder.IsConfigured) {
            optionsBuilder
                .UseNpgsql("Name=MixyBoos")
                .UseSnakeCaseNamingConvention();
            // optionsBuilder.UseSqlite("Name=MixyBoosSqlite");
        }
    }

    protected override void OnModelCreating(ModelBuilder mb) {
        base.OnModelCreating(mb);
        mb.HasAnnotation("Relational:Collation", "en_US.utf8");

        //give the identity tables proper names and schema
        mb.Entity<MixyBoosUser>().ToTable("user", "auth");
        mb.Entity<IdentityUser>().ToTable("identity_user", "auth");
        mb.Entity<IdentityRole>().ToTable("user_role", "auth");
        mb.Entity<IdentityUserClaim<string>>().ToTable("user_claim", "auth");
        mb.Entity<IdentityUserLogin<string>>().ToTable("user_login", "auth");
        mb.Entity<IdentityRoleClaim<string>>().ToTable("role_claim", "auth");
        mb.Entity<IdentityUserToken<string>>().ToTable("user_token", "auth");
        mb.Entity<IdentityUserRole<string>>().ToTable("user_identity_role", "auth");

        mb.Entity<OpenIddictEntityFrameworkCoreApplication>().ToTable($"{IDENTITY_PREFIX}_{"application"}", "auth");
        mb.Entity<OpenIddictEntityFrameworkCoreAuthorization>().ToTable($"{IDENTITY_PREFIX}_{"authorization"}", "auth");
        mb.Entity<OpenIddictEntityFrameworkCoreScope>().ToTable($"{IDENTITY_PREFIX}_{"scope"}", "auth");
        mb.Entity<OpenIddictEntityFrameworkCoreToken>().ToTable($"{IDENTITY_PREFIX}_{"token"}", "auth");
        //end identity stuff

        foreach (var pb in mb.GetColumn("DateCreated")) {
            pb.ValueGeneratedOnAdd()
                .HasDefaultValueSql("now()");
        }

        foreach (var pb in mb.GetColumn("DateUpdated")) {
            pb.ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("now()");
        }
        // foreach (var pb in modelBuilder.GetColumn("Id")) {
        //     pb.ValueGeneratedOnAddOrUpdate()
        //         .HasColumnName("identifier")
        //         .HasColumnType("uuid")
        //         .HasDefaultValueSql("uuid_generate_v4()") // Use 
        //         .IsRequired();
        // }
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default) {
        foreach (var entity in ChangeTracker.Entries()
                     .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                     .Where(e => e.Entity is ISluggedEntity)
                     .Select(e => e.Entity as ISluggedEntity)
                     .Where(e => string.IsNullOrEmpty(e.Slug))) {
            entity.Slug = entity.GenerateSlug(this);
        }

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
