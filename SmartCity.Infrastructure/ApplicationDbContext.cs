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
        public DbSet<CommissionRate> CommissionRates { get; set; }

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
                entity.Property(p => p.CommissionAmount)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");


                entity.HasOne(p => p.CommissionRate)
                      .WithMany(r => r.Properties)
                      .HasForeignKey(p => p.CommissionRateId)
                      .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(p => p.Broker)
                      .WithMany()
                      .HasForeignKey(p => p.BrokerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(x => x.Type)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);
            });

            modelBuilder.Entity<Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });
            });

            modelBuilder.Entity<CommissionRate>(entity =>
            {
                entity.ToTable("CommissionRates");
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Label)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(r => r.MinPrice)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");

                entity.Property(r => r.MaxPrice)
                      .IsRequired()
                      .HasColumnType("decimal(18,2)");

                entity.Property(r => r.RatePercentage)
                      .IsRequired()
                      .HasColumnType("decimal(5,2)");
            });

            modelBuilder.Entity<CommissionRate>().HasData(
                 new CommissionRate
                 {
                     Id = 1,
                     Label = "Below 50 Lakhs",
                     MinPrice = 0,
                     MaxPrice = 4999999,
                     RatePercentage = 2.00m,
                     IsActive = true,
                     CreatedAt = new DateTime(2026, 1, 1)
                 },
                 new CommissionRate
                 {
                     Id = 2,
                     Label = "50 Lakhs to 1 Crore",
                     MinPrice = 5000000,
                     MaxPrice = 10000000,
                     RatePercentage = 1.75m,
                     IsActive = true,
                     CreatedAt = new DateTime(2026, 1, 1)
                 },
                 new CommissionRate
                 {
                     Id = 3,
                     Label = "Above 1 Crore",
                     MinPrice = 10000001,
                     MaxPrice = 9999999999m,
                     RatePercentage = 1.50m,
                     IsActive = true,
                     CreatedAt = new DateTime(2026, 1, 1)
                 }
            );

        }
    }
}