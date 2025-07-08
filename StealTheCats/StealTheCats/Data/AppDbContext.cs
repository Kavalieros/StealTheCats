using Microsoft.EntityFrameworkCore;
using StealTheCats.Models;

namespace StealTheCats.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<CatEntity> Cats => Set<CatEntity>();
        public DbSet<TagEntity> Tags => Set<TagEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CatEntity>()
                .HasMany(c => c.Tags)
                .WithMany(t => t.Cats)
                .UsingEntity(j => j.ToTable("CatTags"));

            modelBuilder.Entity<TagEntity>()
                .HasIndex(t => t.Name)
                .IsUnique();
        }
    }
}
