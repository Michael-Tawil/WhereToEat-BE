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
        public DbSet<Favourite> Favourites { get; set; }

        public DbSet<LastVisited> LastVisited { get; set; }

        public DbSet<Suggested> Suggested { get; set; }

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

            modelBuilder.Entity<Favourite>(entity =>
            {
                entity.ToTable("favourites");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.RestaurantName).HasColumnName("restaurant_name");
                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.Rating).HasColumnName("rating");
                entity.Property(e => e.Cuisine).HasColumnName("cuisine");
                entity.Property(e => e.PriceRange).HasColumnName("price_range");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<LastVisited>(entity =>
            {
                entity.ToTable("last_visited");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.RestaurantName).HasColumnName("restaurant_name");
                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.Rating).HasColumnName("rating");
                entity.Property(e => e.Cuisine).HasColumnName("cuisine");
                entity.Property(e => e.PriceRange).HasColumnName("price_range");
                entity.Property(e => e.VisitedAt).HasColumnName("visited_at").ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Suggested>(entity =>
            {
                entity.ToTable("suggested");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.RestaurantName).HasColumnName("restaurant_name");
                entity.Property(e => e.SuggestedAt).HasColumnName("suggested_at").ValueGeneratedOnAdd();
            });
        }
    }
}
