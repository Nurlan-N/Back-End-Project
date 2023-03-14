using Back_End_Project.DataAccessLayer;
using Back_End_Project.Models;
using Back_End_Project.ViewModels.ShopViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Back_End_Project.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ShopVM vm = new()
            {
                Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync(),
                Products = await _context.Products.Where(p => p.IsDeleted == false).ToListAsync(),
            };
            return View(vm);
        }
        [HttpGet]
        public async Task<IActionResult> CategoryFilte(int? categoryId)
        {


            ShopVM vm = new()
            {
                Categories = await _context.Categories.Where(c => c.IsDeleted == false).ToListAsync(),
                Products = await _context.Products.Where(p => p.IsDeleted == false && p.CategoryId == categoryId).ToListAsync(),
            };
            return View(vm);
        }
    }
}
