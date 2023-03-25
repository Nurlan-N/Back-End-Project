using Back_End_Project.DataAccessLayer;
using Back_End_Project.Extentions;
using Back_End_Project.Helpers;
using Back_End_Project.Models;
using Back_End_Project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Back_End_Project.Areas.Manage.Controllers
{
    [Area ("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BlogController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int pageIndex = 1)
        {
            IQueryable<Blog?> blogs = _context.Blogs.Where(p => p.IsDeleted == false);

            return View(PageNatedList<Blog>.Create(blogs, pageIndex, 5));
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Blog blog)
        {
            ViewBag.Tags = await _context.Tags.Where(b => b.IsDeleted == false).ToListAsync();
            if (!ModelState.IsValid) return View(blog);

            


            if (blog.ImageFile != null)
            {
                if (!blog.ImageFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("ImageFile", "ImageFile File Yalniz JPG Formatda ola biler");
                    return View(blog);
                }
                if (!blog.ImageFile.CheckFileLength(300))
                {
                    ModelState.AddModelError("ImageFile", "ImageFile File Yalniz 300Kb  ola biler");
                    return View(blog);
                }
                blog.Image = await blog.ImageFile.CreateFileAsync(_webHostEnvironment, "assets", "img", "blog");
            }
            else
            {
                ModelState.AddModelError("ImageFile", "ImageFile File mutleqdir");
                return View(blog);
            }
            blog.CreatedAt = DateTime.UtcNow.AddHours(4);
            blog.CreatedBy = "Admin";

            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) { return BadRequest(); }
            Blog blog = await _context.Blogs
                .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (blog == null) { return NotFound(); }

            return View(blog);
        }
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Blog blog = await _context.Blogs
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
            if (blog == null) return NotFound();



            return View(blog);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, Blog blog)
        {

            if (!ModelState.IsValid) return View();

            if (id == null || id != blog.Id) return BadRequest();

            Blog blogDb = await _context.Blogs
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (blogDb == null) return NotFound();

            //StartImageFile
            if (blog.ImageFile != null)
            {
                if (!blog.ImageFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", "Main File Yalniz JPG Formatda ola biler");
                    return View(blog);
                }
                if (!blog.ImageFile.CheckFileLength(300))
                {
                    ModelState.AddModelError("MainFile", "Main File Yalniz 300Kb  ola biler");
                    return View(blog);
                }
                FileHelpers.DeleteFile(blogDb.Image, _webHostEnvironment, "assets", "img", "blog");

                blogDb.Image = await blog.ImageFile.CreateFileAsync(_webHostEnvironment, "assets", "img", "blog");
            }
            if (blog.Title != null) { blogDb.Title = blog.Title; }
            if (blog.Description != null) { blogDb.Description = blog.Description; }

            blogDb.UpdatetAt = DateTime.UtcNow.AddDays(4);
            blogDb.UpdatetBy = "Admin";

            await _context.SaveChangesAsync();
           


            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) { return BadRequest(); }

            Blog blog = await _context.Blogs.FirstOrDefaultAsync(b => b.IsDeleted == false && b.Id == id);

            if (blog == null) { return NotFound(); }

            blog.IsDeleted = true;
            blog.DeletedBy = "System";
            blog.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return PartialView("_ProductIndexPartial");
        }
    }
}
