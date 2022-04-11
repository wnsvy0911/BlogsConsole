using System;
using NLog.Web;
using System.IO;
using System.Linq;

namespace BlogsConsole
{
    class Program
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
            if (name != "") {
                logger.Info("Blog added - {name}", name);
                return new Blog { Name = name };
            } else {
                return null;
            }
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
            if (query.Count() > 0) {
                Console.WriteLine(query.Count() + " post(s) returned");
                foreach (var item in query)
                {
                    Console.WriteLine("Title: " + item.Title);
                    Console.WriteLine("Content: " + item.Content + "\n");
                }
            } else {
                Console.WriteLine("Total Number Of Posts: 0");
            }
        }

        static Post createPost(BloggingContext db, Blog blog) {
            Console.Write("Enter the Post title: ");
            var title = Console.ReadLine();
            Console.Write("Enter the Post content: ");
            var content = Console.ReadLine();
            if (title != "") {
                return new Post {BlogId = blog.BlogId, Title = title, Content = content, Blog = blog };
            } else {
                logger.Error("Post title cannot be null");     
                return null;
            }            

        }

        static Blog postPreWorkflow(BloggingContext db) {
            Console.WriteLine("Select the blog you would to post to: ");
            displayAllBlogs(db, true);
            var response = Console.ReadLine();
            if (int.TryParse(response, out int blogId))
            {
                if (blogId == 0) {
                    listAllBlogsAllPosts(db);
                    return null;
                } else {
                    return getBlogById(db, blogId);
                }
            }
            logger.Error("Invalid Blog Id");
            return null;
        }

        static void displayAllBlogs(BloggingContext db, bool withId) {
            var query = db.Blogs.OrderBy(b => b.Name);

            Console.WriteLine("Select the blog's posts to display:");
            Console.WriteLine("0) Posts from all blogs");
      
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

        static void listAllBlogsAllPosts(BloggingContext db) {
            var query = db.Blogs.OrderBy(b => b.Name).ToList();
            foreach (var blog in query)
            {
                listPostsByBlogId(db, blog);
            }
        }

        static bool isUniqueBlog(BloggingContext db, Blog blog) {
            var query = db.Blogs.OrderBy(b => b.Name);
            foreach (var blogItem in query)
            {
                if (blogItem.Name == blog.Name) {
                    logger.Error("Please Choose a Unique Blog Name");
                    return false;
                }
            }
            return true;
        }
        static void Main(string[] args)
        {
            logger.Info("Program started");
            bool run = true;
            while(run) {
                try
                {
                    var db = new BloggingContext();
                    string choice = mainMenu();
                    Blog blog;
                    switch (choice)
                    {
                        case "1":
                            logger.Info("Option 1 selected");
                            displayAllBlogs(db, false);
                            break;
                        case "2":
                            logger.Info("Option 2 selected");
                            blog = createBlogWorkflow();
                            if(blog != null && isUniqueBlog(db, blog)){
                                db.AddBlog(blog);
                            } else {
                                logger.Error("Blog name cannot be null");
                            }
                            break;
                        case "3":
                            logger.Info("Option 3 selected");
                            blog = postPreWorkflow(db);
                            if (blog != null) {
                                var post = createPost(db, blog);
                                if (post != null) {
                                    db.AddPost(post);
                                    logger.Info("Added Post");                                 
                                }
                            }
                            break;
                        case "4":
                            logger.Info("Option 4 selected");
                            blog = postPreWorkflow(db);
                            
                            if (blog != null) {
                                listPostsByBlogId(db, blog);
                            }                            
                            break;
                        case "q":
                            run = false;
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
            }

            logger.Info("Program ended");
      
        }
    }
}