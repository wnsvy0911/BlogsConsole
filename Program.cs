using System;
using NLog.Web;
using System.IO;
using System.Linq;

namespace BlogsConsole
{
    class Program
    // create static instance of Logger
    {    
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
    

        static string mainMenu() {
            Console.WriteLine("Enter your selection:");
            Console.WriteLine("1) Display all blogs");
            Console.WriteLine("2) Add Blog");
            Console.WriteLine("3) Create Post");
            Console.WriteLine("4) Display Posts");
            Console.WriteLine("Enter q to quit");
            return Console.ReadLine();
        }

        static Blog createBlogWorkflow() {
            Console.Write("Enter a name for a new Blog: ");
            var name = Console.ReadLine();
            logger.Info("Blog added - {name}", name);
            return new Blog { Name = name };
        }

        static Blog getBlogById(BloggingContext db, int blogId) {
            Blog blog = db.Blogs.FirstOrDefault(b => b.BlogId == blogId);
            if (blog != null)
            {
                return blog;
            }
            logger.Error("Blog Not Found: " + blogId);
            return null;
        }

        static void listPostsByBlogId(BloggingContext db, Blog blog) {
            var query = db.Posts.Where(p => p.BlogId == blog.BlogId).ToList();
            Console.WriteLine("Blog: " + blog.Name);
            Console.WriteLine("Total Number Of Posts: " + query.Count());
            foreach (var item in query)
            {
                Console.WriteLine(item.PostId + ") " + item.Title);
            }
        }

        static Post createPost(BloggingContext db, Blog blog) {
            Console.Write("Enter the Post title: ");
            var title = Console.ReadLine();
            Console.Write("Enter the Post content: ");
            var content = Console.ReadLine();
            return new Post { BlogId = blog.BlogId, Title = title, Content = content, Blog = blog };
        }

        static Blog postPreWorkflow(BloggingContext db) {
            Console.WriteLine("Select the blog you would to post to: ");
            displayAllBlogs(db, true);
            var response = Console.ReadLine();
            if (int.TryParse(response, out int blogId))
            {
                return getBlogById(db, blogId);
            }
            logger.Error("Not A Valid Blog Id.");
            return null;
        }

        static void displayAllBlogs(BloggingContext db, bool withId) {
            var query = db.Blogs.OrderBy(b => b.Name);

            Console.WriteLine("Select the blog's posts to display:");
            Console.WriteLine("Total Number Of Blogs: " + query.Count());
            foreach (var item in query)
            {
                if (withId) {
                    Console.WriteLine(item.BlogId + ") " + item.Name);
                } else {
                    Console.WriteLine(item.Name);
                }
            }
        }
        static void Main(string[] args)
        {
            logger.Info("Program started");

            try
            {
                var db = new BloggingContext();
                string choice = mainMenu();
                Blog blog;
                switch (choice)
                {
                    case "1":
                    // Display All Blogs, pass in database context
                        displayAllBlogs(db, false);
                    //  displayAllBlogs(db, true);
                        break;
                    case "2":
                    // Add Blog
                        db.AddBlog(createBlogWorkflow());
                        break;
                    case "3":
                    // Create Post
                        blog = postPreWorkflow(db);
                        var post = createPost(db, blog);
                        db.Posts.Add(post);
                        break;
                    case "4":
                    // Display Posts
                        blog = postPreWorkflow(db);
                        listPostsByBlogId(db, blog);
                        break;
                    case "q":
                    // Exit 
                        break;
                    default:
                        Console.WriteLine("Please Enter A Valid Option.");
                        break;
                }

            }
            
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            logger.Info("Program ended");
        
        }
    }
}