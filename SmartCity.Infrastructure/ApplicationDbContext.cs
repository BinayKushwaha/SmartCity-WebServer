using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartCity.Domain;

namespace SmartCity.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<RetailProperty> Properties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RetailProperty>().ToTable("Properties");
            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            modelBuilder.Entity<RetailProperty>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Type).IsRequired();
                entity.Property(p => p.Location).IsRequired().HasMaxLength(200);
                entity.Property(p => p.Price).IsRequired();
                entity.Property(p => p.Features).HasMaxLength(500);
            });

            modelBuilder.Entity<Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });
            });

        }
    }
}