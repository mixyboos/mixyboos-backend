using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MixyBoos.Api.Data.Models;
using MixyBoos.Api.Data.Options;
using MixyBoos.Api.Data.Utils;
using MixyBoos.Api.Services.Extensions;

namespace MixyBoos.Api.Data;

public class MixyBoosContext : IdentityDbContext<MixyBoosUser, IdentityRole<Guid>, Guid> {
  private readonly DbScaffoldOptions _settings;
  private readonly ILogger<MixyBoosContext> _logger;
  public DbSet<Mix> Mixes { get; set; }

  public DbSet<MixPlay> MixPlays { get; set; }
  public DbSet<MixLike> MixLikes { get; set; }
  public DbSet<MixShare> MixShares { get; set; }
  public DbSet<MixDownload> MixDownloads { get; set; }

  public DbSet<LiveShow> LiveShows { get; set; }
  public DbSet<Tag> Tags { get; set; }
  public DbSet<ShowChat> ShowChat { get; set; }


  public MixyBoosContext(DbContextOptions<MixyBoosContext> options, IOptions<DbScaffoldOptions> settings,
    ILogger<MixyBoosContext> logger)
    : base(options) {
    _settings = settings.Value;
    _logger = logger;
  }

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
        //TODO: Re-enable this once
        // https://github.com/efcore/EFCore.NamingConventions/issues/209
        // is resolved
        // .UseSnakeCaseNamingConvention()
        ;
    }
  }

  protected override void OnModelCreating(ModelBuilder mb) {
    base.OnModelCreating(mb);

    mb.HasDefaultSchema("mixyboos");
    mb.UseIdentityByDefaultColumns();

    mb.HasAnnotation("Relational:Collation", "en_US.utf8");

    //give the identity tables proper names and schema
    mb.Entity<MixyBoosUser>().ToTable("user", "oid");
    mb.Entity<IdentityUser>().ToTable("identity_user_base", "oid");
    mb.Entity<IdentityUser<Guid>>().ToTable("identity_user", "oid");
    mb.Entity<IdentityRole<Guid>>().ToTable("user_user_role", "oid");
    mb.Entity<IdentityUserClaim<Guid>>().ToTable("user_claim", "oid");
    mb.Entity<IdentityUserLogin<Guid>>().ToTable("user_login", "oid");
    mb.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claim", "oid");
    mb.Entity<IdentityUserToken<Guid>>().ToTable("user_token", "oid");
    mb.Entity<IdentityUserRole<Guid>>().ToTable("user_identity_role", "oid");
    //end identity stuff

    mb.Entity<Mix>().ToTable("mixes");
    mb.Entity<LiveShow>().ToTable("live_shows");
    mb.Entity<ShowChat>().ToTable("show_chats");
    mb.Entity<Tag>().ToTable("tags");
    mb.Entity<MixPlay>().ToTable("mix_plays");
    mb.Entity<MixLike>().ToTable("mix_likes");
    mb.Entity<MixShare>().ToTable("mix_shares");
    mb.Entity<MixDownload>().ToTable("mix_download");

    foreach (var pb in __getColumns(mb, "DateCreated")) {
      pb.ValueGeneratedOnAdd()
        .HasDefaultValueSql("now()");
    }

    foreach (var pb in __getColumns(mb, "DateUpdated")) {
      pb.ValueGeneratedOnAddOrUpdate()
        .HasDefaultValueSql("now()");
    }

    foreach (var pb in __getColumns(mb, "Id")) {
      pb.IsRequired()
        .HasConversion<string>()
        .ValueGeneratedOnAdd();
    }

    mb.Entity<MixyBoosUser>()
      .HasMany(p => p.Following)
      .WithMany(p => p.Followers)
      .UsingEntity(j => j.ToTable("user_followers"));

    mb.Entity<Mix>()
      .HasMany(m => m.Tags)
      .WithMany()
      .UsingEntity(j => j.ToTable("mix_tags"));

    mb.Entity<LiveShow>()
      .HasMany(m => m.Tags)
      .WithMany()
      .UsingEntity(j => j.ToTable("show_tags"));

    mb.Entity<Mix>()
      .HasMany(m => m.Plays)
      .WithOne(m => m.Mix);

    mb.Entity<MixPlay>()
      .HasKey(i => new { i.MixId, i.UserId });

    mb.Entity<Mix>()
      .Navigation(m => m.Plays)
      .AutoInclude();

    mb.Entity<Mix>()
      .Navigation(m => m.User)
      .AutoInclude();

    mb.SeedAuthenticationUsers(_settings);
  }

  public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
    CancellationToken cancellationToken = default) {
    try {
      foreach (var entity in ChangeTracker.Entries()
                 .Where(e => e.State is EntityState.Added or EntityState.Modified)
                 .Where(e => e.Entity is ISluggedEntity)
                 .Select(e => e.Entity as ISluggedEntity)
                 .Where(e => string.IsNullOrEmpty(e.Slug))) {
        entity.Slug = entity.GenerateSlug(this);
      }

      return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    } catch (DbUpdateConcurrencyException ex) {
      Console.WriteLine(ex.Message);
      throw;
    }
  }
}
