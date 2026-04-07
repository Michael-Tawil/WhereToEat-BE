using Microsoft.EntityFrameworkCore;
using WhereToEat_BE.Models;

namespace WhereToEat_BE.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.Property(e=>e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").ValueGeneratedOnAdd();
            });
        }
    }
}
