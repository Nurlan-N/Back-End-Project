using Back_End_Project.DataAccessLayer;
using Back_End_Project.Models;
using Back_End_Project.ViewModels;
using Back_End_Project.ViewModels.BlogViewModels;
using Back_End_Project.ViewModels.ProductVIewsModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Back_End_Project.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BlogController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index(int pageIndex = 1)
        {
            IQueryable<Blog?> blogs = _context.Blogs.Where(b => b.IsDeleted == false);

            return View(PageNatedList<Blog>.Create(blogs, pageIndex, 6));
        }
        public async Task<IActionResult> Detail(int id)
        {
            ViewBag.Blog = await _context.Blogs.Where(b => b.IsDeleted == false).ToListAsync(); 
            if (id == null) { return BadRequest(); }
            Blog blog = await _context.Blogs
                .Include(b => b.Comments.Where(r => !r.IsDeleted))
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (blog == null) { return NotFound(); }
            BlogReviewVM blogReviewVM = new()
            {
                Blog = blog,
                Comment = new Comment { BlogId = id }
            };
            return View(blogReviewVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            Blog blog = await _context.Blogs
                .Include(b => b.Comments.Where(r => !r.IsDeleted))
                .FirstOrDefaultAsync(b => b.Id == comment.BlogId && !b.IsDeleted);


            BlogReviewVM blogReviewVM = new BlogReviewVM
            {
                Blog = blog,
                Comment = comment
            };

            if (!ModelState.IsValid) return RedirectToAction("Detail", blogReviewVM);
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            comment.UserId = appUser.Id;
            comment.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            comment.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Detail), new { id = blog.Id });
        }
    }
}
