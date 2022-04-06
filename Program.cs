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
            Console.Write("Enter your selection:");
            Console.Write("1) Display all blogs");
            Console.Write("2) Add Blog");
            Console.Write("3) Create Post");
            Console.Write("4) Display Posts");
            Console.Write("Enter q to quit");

            return Console.ReadLine();
        }

        static Blog createBlogWorkflow() {
            Console.Write("Enter a name for a new Blog: ");
            var name = Console.ReadLine();
            logger.Info("Blog added - {name}", name);
            return new Blog { Name = name };
        }

        static Post createPostWorkflow(BloggingContext db) {

            Console.WriteLine("Please Choose A Blog To Post To.");
            displayAllBlogs(db, true);
            var blogChoice = Console.ReadLine();

            Console.Write("Enter a title for the new Post:");
            var title = Console.ReadLine();
            Console.Write("Enter content for the new Post");
            var content = Console.ReadLine();
            return new Post { Title = title, Content = content };
        }

        static void displayAllBlogs(BloggingContext db, bool withId) {
            var query = db.Blogs.OrderBy(b => b.Name);

            Console.WriteLine("All blogs in the database:");
            foreach (var item in query)
            {
                Console.WriteLine(item.Name);
            }
        }
        static void Main(string[] args)
        {
            logger.Info("Program started");

            try
            {
                var db = new BloggingContext();
                string choice = mainMenu();
                switch (choice)
                {
                    case "1":
                    // Display All Blogs, pass in database context
                        displayAllBlogs(db, false);
                        displayAllBlogs(db, true);
                        break;
                    case "2":
                    // Add Blog
                        db.AddBlog(createBlogWorkflow());
                        break;
                    case "3":
                    // Create Post
                        break;
                    case "4":
                    // Display Posts
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