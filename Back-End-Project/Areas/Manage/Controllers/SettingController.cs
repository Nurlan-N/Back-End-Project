using Back_End_Project.DataAccessLayer;
using Back_End_Project.Models;
using Back_End_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Back_End_Project.Areas.Manage.Controllers
{
    [Area("manage")]
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
    }
}
