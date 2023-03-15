using Back_End_Project.DataAccessLayer;
using Back_End_Project.Models;
using Back_End_Project.ViewModels;
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

        public async Task<IActionResult> Index(int? categoryId, int pageIndex = 1)
        {
            
            ShopVM vm = new()
            {
                Categories = await _context.Categories.Include(c => c.Products.Where(p => p.IsDeleted == false)).Where(c => c.IsDeleted == false).ToListAsync(),
                Products = PageNatedList<Product>.Create(_context.Products.Where(p => (categoryId != null ? p.CategoryId == categoryId : true) && p.IsDeleted == false),pageIndex,12)
            };           
            return View(vm);
        }
        
        public async Task<IActionResult> List(string? range = "")
        {
            double minValue = 0;
            double maxValue = 0;

            if (range != "")
            {
                string[] arr = range.Split(' ');
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = arr[i].Substring(1);
                }
                minValue = double.Parse(arr[0]);
                maxValue = double.Parse(arr[2]);
            }
            IEnumerable<Product> product = await _context.Products.Where(p => p.IsDeleted == false && (p.Price >= minValue && p.Price <= (maxValue == 0 ? 400 : maxValue))).ToListAsync();

            return PartialView("_ShopListPartial", product);
        }
    }
}
