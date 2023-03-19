using Back_End_Project.DataAccessLayer;
using Back_End_Project.Models;
using Back_End_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Back_End_Project.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;

        public BlogController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int pageIndex = 1)
        {
            IQueryable<Blog?> blogs = _context.Blogs.Where(b => b.IsDeleted == false);

            return View(PageNatedList<Blog>.Create(blogs, pageIndex, 6));
        }
    }
}
