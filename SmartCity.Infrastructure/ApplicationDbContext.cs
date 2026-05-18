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

            modelBuilder.Entity<RetailProperty>().HasData(
            new RetailProperty
            {
                Id = 1,
                Type = PropertyType.Residential,
                Location = "Baneshwor, Kathmandu",
                Price = 4500000m, // 45 Lakhs -> Slab 1 (2.00%)
                Features = "Cozy 2BHK apartment with city views and 24/7 water supply.",
                BrokerId = "a4392244-a556-4af8-94e9-e80725cbc880",
                CommissionRateId = 1,
                CommissionAmount = 90000.00m, // 4,500,000 * 0.02
                CreatedAt = new DateTime(2026, 1, 15, 10, 0, 0, DateTimeKind.Utc)
            },
            new RetailProperty
            {
                Id = 2,
                Type = PropertyType.Commercial,
                Location = "Durbar Marg, Kathmandu",
                Price = 85000000m, // 8.5 Crore -> Slab 3 (1.50%)
                Features = "Premium office space on the ground floor with high foot traffic.",
                BrokerId = "a4392244-a556-4af8-94e9-e80725cbc880",
                CommissionRateId = 3,
                CommissionAmount = 1275000.00m, // 85,000,000 * 0.015
                CreatedAt = new DateTime(2026, 2, 1, 14, 30, 0, DateTimeKind.Utc)
            },
            new RetailProperty
            {
                Id = 3,
                Type = PropertyType.Industrial,
                Location = "Patandhoka, Lalitpur",
                Price = 150000000m, // 15 Crore -> Slab 3 (1.50%)
                Features = "Large manufacturing warehouse equipped with a 3-phase power supply.",
                BrokerId = "a4392244-a556-4af8-94e9-e80725cbc880",
                CommissionRateId = 3,
                CommissionAmount = 2250000.00m, // 150,000,000 * 0.015
                CreatedAt = new DateTime(2026, 3, 10, 9, 15, 0, DateTimeKind.Utc)
            },
            new RetailProperty
            {
                Id = 4,
                Type = PropertyType.Agricultural,
                Location = "Jhapa, Nepal",
                Price = 3500000m, // 35 Lakhs -> Slab 1 (2.00%)
                Features = "Highly fertile multi-crop farming land spanning 5 Ropanis with access road.",
                BrokerId = "a4392244-a556-4af8-94e9-e80725cbc880",
                CommissionRateId = 1,
                CommissionAmount = 70000.00m, // 3,500,000 * 0.02
                CreatedAt = new DateTime(2026, 4, 5, 11, 0, 0, DateTimeKind.Utc)
            },
            new RetailProperty
            {
                Id = 5,
                Type = PropertyType.Residential,
                Location = "Jhamsikhel, Lalitpur",
                Price = 7500000m, // 75 Lakhs -> Slab 2 (1.75%)
                Features = "Modern modular flat layout close to prime hub restaurant spaces.",
                BrokerId = "a4392244-a556-4af8-94e9-e80725cbc880",
                CommissionRateId = 2,
                CommissionAmount = 131250.00m, // 7,500,000 * 0.0175
                CreatedAt = new DateTime(2026, 4, 22, 16, 45, 0, DateTimeKind.Utc)
            },
            new RetailProperty
            {
                Id = 6,
                Type = PropertyType.Commercial,
                Location = "New Road, Pokhara",
                Price = 9000000m, // 90 Lakhs -> Slab 2 (1.75%)
                Features = "Multi-story retail showroom block located in a prime commercial intersection.",
                BrokerId = "a4392244-a556-4af8-94e9-e80725cbc880",
                CommissionRateId = 2,
                CommissionAmount = 157500.00m, // 9,000,000 * 0.0175
                CreatedAt = new DateTime(2026, 5, 2, 13, 20, 0, DateTimeKind.Utc)
            },
            new RetailProperty
            {
                Id = 7,
                Type = PropertyType.Residential,
                Location = "Chabahil, Kathmandu",
                Price = 18500000m, // 1.85 Crore -> Slab 3 (1.50%)
                Features = "Semi-furnished family house featuring parking spaces for 1 car and 3 motorbikes.",
                BrokerId = "a4392244-a556-4af8-94e9-e80725cbc880",
                CommissionRateId = 3,
                CommissionAmount = 277500.00m, // 18,500,000 * 0.015
                CreatedAt = new DateTime(2026, 5, 14, 10, 0, 0, DateTimeKind.Utc)
            }
        );

        }
    }
}