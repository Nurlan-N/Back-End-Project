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
    public class SettingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SettingController(AppDbContext context, IWebHostEnvironment webHostEnvironment = null)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int pageIndex = 1)
        {
            IQueryable<Setting> settings = _context.Settings;

            return View(PageNatedList<Setting>.Create(settings, pageIndex, 3));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) { return BadRequest(); }
            Setting setting = await _context.Settings.FirstOrDefaultAsync(c => c.Id == id );

            if (setting == null) { return NotFound(); }

            ViewBag.Settings = await _context.Settings.ToListAsync();


            return View(setting);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, Setting setting)
        {
            ViewBag.Settings = await _context.Settings.ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(setting);
            }
            if (id == null) { return BadRequest(); }
            if (id != setting.Id) { return BadRequest(); }

            Setting settingDb = await _context.Settings.FirstOrDefaultAsync( s => s.Id == id);
            if (settingDb == null) { return NotFound(); }


            settingDb.Value = setting.Value.Trim();

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
       
    }
}
