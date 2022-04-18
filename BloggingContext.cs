using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BlogsConsole
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        public void AddBlog(Blog blog)
        {
            this.Blogs.Add(blog);
            this.SaveChanges();
        }

        public void DeleteBlog(Blog blog)
        {
            this.Blogs.Remove(blog);
            this.SaveChanges();
        }

        public void EditBlog(Blog UpdatedBlog)
        {
            Blog blog = this.Blogs.Find(UpdatedBlog.BlogId);
            blog.Name = UpdatedBlog.Name;
            this.SaveChanges();
        }

        public void AddPost(Post post)
        {
            this.Posts.Add(post);
            this.SaveChanges();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            optionsBuilder.UseSqlServer(@config["BloggingContext:ConnectionString"]);
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}