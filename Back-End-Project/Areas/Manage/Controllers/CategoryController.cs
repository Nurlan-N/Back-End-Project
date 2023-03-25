using Back_End_Project.DataAccessLayer;
using Back_End_Project.Helpers;
using Back_End_Project.Models;
using Back_End_Project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Back_End_Project.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            IQueryable<Category> categories = _context.Categories.Include(c => c.Products)
                .Where(c => c.IsDeleted == false );
            return View(PageNatedList<Category>.Create(categories, pageIndex, 3));
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            ViewBag.Categories = await _context.Categories.Include(c => c.Products).Where(c => c.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(category);
            }
            if (await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Name.ToLower().Contains(category.Name.Trim().ToLower())))
            {
                ModelState.AddModelError("Name", "Bu adda category var");
                return View(category);
            }

            category.Name = category.Name.Trim();
            category.CreatedAt = DateTime.UtcNow.AddHours(4);
            category.CreatedBy = "System";

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();


            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) { return BadRequest(); }
             Category category = await _context.Categories
                .Include(c => c.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (category == null) { return NotFound(); }

            return View(category);
        }
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) { return BadRequest(); }
            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (category == null) { return NotFound(); }

            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync();


            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsDeleted == false ).ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(category);
            }
            if (id == null) { return BadRequest(); }
            if (id != category.Id) { return BadRequest(); }

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);
            if (dbCategory == null) { return NotFound(); }

            
            dbCategory.Name = category.Name.Trim();
            dbCategory.UpdatetBy = "System";
            dbCategory.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (category == null) return NotFound();

            return View(category);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories
                .Include(c => c.Products.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.IsDeleted == false && c.Id == id);

            if (category == null) return NotFound();

            category.IsDeleted = true;
            category.DeletedAt = DateTime.UtcNow.AddHours(4);
            category.DeletedBy = "System";

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
