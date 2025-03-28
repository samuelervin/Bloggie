using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Data
{
    public class BloggingDbContext : DbContext
    {
        public BloggingDbContext(DbContextOptions<BloggingDbContext> options) : base(options)
        {
        }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogPost>()
                .HasMany(p => p.Tags)
                .WithMany(p => p.BlogPosts)
                .UsingEntity(j => j.ToTable("BlogPostTags"));
        }
    }
}
