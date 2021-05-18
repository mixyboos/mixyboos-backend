using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using mixyboos_api.Data;
using MixyBoos.Api.Services.Extensions;

#nullable disable

namespace MixyBoos.Api {
    public partial class MixyBoosContext : IdentityDbContext<ApplicationUser> {
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
            OnModelCreatingPartial(modelBuilder);

            foreach (var pb in modelBuilder.GetColumn("CreateDate")) {
                pb.ValueGeneratedOnAdd()
                    .HasDefaultValueSql("getdate()");
            }

            foreach (var pb in modelBuilder.GetColumn("UpdateDate")) {
                pb.ValueGeneratedOnAddOrUpdate()
                    .HasDefaultValueSql("getdate()");
            }
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
