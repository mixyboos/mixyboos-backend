using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Services.Extensions;
using MixyBoos.Api.Services.Helpers;

#nullable disable

namespace MixyBoos.Api.Data {
    public class MixyBoosContext : IdentityDbContext<ApplicationUser> {
        public DbSet<Mix> Mixes { get; set; }

        public MixyBoosContext() {
        }

        public MixyBoosContext(DbContextOptions<MixyBoosContext> options)
            : base(options) {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseNpgsql("Name=MixyBoosContext");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasAnnotation("Relational:Collation", "en_US.utf8");

            foreach (var pb in modelBuilder.GetColumn("DateCreated")) {
                pb.ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NOW()");
            }

            foreach (var pb in modelBuilder.GetColumn("DateUpdated")) {
                pb.ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("NOW()");
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
}
