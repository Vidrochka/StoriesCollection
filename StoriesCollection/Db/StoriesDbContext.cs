using Microsoft.EntityFrameworkCore;
using StoriesCollection.Db.Models;
using System;

namespace StoriesCollection.Db
{
    public class StoriesDbContext : DbContext
    {
        public DbSet<Story> Stories {  get; set;}
        public DbSet<StoryPart> StoryParts { get; set; }
        public DbSet<Button> Buttons { get; set; }

        public StoriesDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("env DB_CONNECTION_STRING required");

            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Story>(entity => {
                entity.HasIndex(e => e.Name).IsUnique(true);
            });

            builder.Entity<StoryPart>(entity => {
                entity.HasIndex(e => e.StoryId).IsUnique(false);
            });

            builder.Entity<Button>(entity => {
                entity.HasIndex(e => new { e.SourceStoryPartId, e.DestinationStoryPartId }).IsUnique(false);
            });

            builder.Entity<Button>().HasOne(x => x.SourceStoryPart).WithMany(x => x.ButtonsNext);
            builder.Entity<Button>().HasOne(x => x.DestinationStoryPart).WithMany(x => x.ButtonsFrom);
        }
    }
}
