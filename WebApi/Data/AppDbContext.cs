using Microsoft.EntityFrameworkCore;
using SharedLib.Models;

namespace WebApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình bảng Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique(); // Mã phải unique
                entity.HasIndex(e => e.Email).IsUnique(); // Email phải unique
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Address).IsRequired().HasMaxLength(250);
            });

            // Mock Data
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Code = "NV001",
                    FullName = "Nguyễn Văn A",
                    DateOfBirth = new DateTime(1990, 1, 15),
                    Email = "nguyenvana@example.com",   
                    Phone = "0912345678",
                    Address = "123 Đường ABC, Quận 1, TP.HCM"
                },
                new User
                {
                    Id = 2,
                    Code = "NV002",
                    FullName = "Trần Thị B",
                    DateOfBirth = new DateTime(1995, 5, 20),
                    Email = "tranthib@example.com",
                    Phone = "0987654321",
                    Address = "456 Đường XYZ, Quận 3, TP.HCM"
                }
            );
        }
    }
}