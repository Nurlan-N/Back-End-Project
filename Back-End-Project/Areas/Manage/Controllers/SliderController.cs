using Back_End_Project.DataAccessLayer;
using Back_End_Project.Extentions;
using Back_End_Project.Models;
using Back_End_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;

namespace Back_End_Project.Areas.Manage.Controllers
{
    [Area("manage")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SliderController(AppDbContext context, IWebHostEnvironment webHostEnvironment = null)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index(int pageIndex = 1)
        {
            IQueryable<Slider> sliders = _context.Sliders.Where(s => s.IsDeleted == false);

            return View(PageNatedList<Slider>.Create(sliders, pageIndex, 3));
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsDeleted == false).ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Slider slider)
        {
            ViewBag.Sliders = await _context.Sliders
                .Where(c => c.IsDeleted == false).ToListAsync();
            if (!ModelState.IsValid) return View(slider);

            if (slider.SubTitle == null)
            {
                ModelState.AddModelError("SubTitle", "SubTitle Boş ola bilməz...!!!");
                return View(slider);
            }
            if (slider.Title == null)
            {
                ModelState.AddModelError("Title", "Title Boş ola bilməz...!!!");
                return View(slider);
            }
            if (slider.Description == null)
            {
                ModelState.AddModelError("Description", "Description Boş ola bilməz...!!!");
                return View(slider);
            }

            if (slider.ImageFile != null)
            {
                if (!slider.ImageFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("ImageFile", "ImageFile File Yalniz JPG Formatda ola biler");
                    return View(slider);
                }
                if (!slider.ImageFile.CheckFileLength(300))
                {
                    ModelState.AddModelError("ImageFile", "ImageFile File Yalniz 300Kb  ola biler");
                    return View(slider);
                }
                slider.Image = await slider.ImageFile.CreateFileAsync(_webHostEnvironment, "assets", "img", "slider");
            }
            else
            {
                ModelState.AddModelError("ImageFile", "ImageFile File mutleqdir");
                return View(slider);
            }
            slider.CreatedAt = DateTime.UtcNow.AddDays(4);
            slider.CreatedBy = "Admin";

            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Slider slider = await _context.Sliders
                .FirstOrDefaultAsync(s => s.Id == id && s.IsDeleted == false);
            if (slider == null) return NotFound();

            ViewBag.Sliders = await _context.Sliders
                .Where(c => c.IsDeleted == false).ToListAsync();


            return View(slider);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id , Slider slider)
        {
            ViewBag.Sliders = await _context.Sliders
                           .Where(c => c.IsDeleted == false).ToListAsync();
            if (!ModelState.IsValid) return View(slider);
            Slider sliderDb = await _context.Sliders
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (sliderDb == null) return NotFound();
            if (id == null || id != slider.Id) return BadRequest();

            if (slider.ImageFile != null)
            {
                if (!slider.ImageFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("ImageFile", "ImageFile File Yalniz JPG Formatda ola biler");
                    return View(slider);
                }
                if (!slider.ImageFile.CheckFileLength(300))
                {
                    ModelState.AddModelError("ImageFile", "ImageFile File Yalniz 300Kb  ola biler");
                    return View(slider);
                }
                slider.Image = await slider.ImageFile.CreateFileAsync(_webHostEnvironment, "assets", "img", "slider");
            }
            if (slider.SubTitle != null) { sliderDb.SubTitle = slider.SubTitle; }
            if (slider.Title != null) { sliderDb.Title = slider.Title; }
            if (slider.Description != null) { sliderDb.Description = slider.Description; }

            slider.UpdatetAt = DateTime.UtcNow.AddDays(4);
            slider.UpdatetBy = "Admin";

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id, int pageIndex = 1)
        {
            if (id == null) { return BadRequest(); }

            Slider slider = await _context.Sliders.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (slider == null) { return NotFound(); }

            slider.IsDeleted = true;
            slider.DeletedBy = "System";
            slider.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            IQueryable<Slider> sliders = _context.Sliders.Where(b => b.IsDeleted == false).OrderByDescending(b => b.Id);


            return View("index", PageNatedList<Slider>.Create(sliders, pageIndex, 3));
        }
    }
}
