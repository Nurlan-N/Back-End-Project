using Back_End_Project.DataAccessLayer;
using Back_End_Project.Models;
using Back_End_Project.ViewModels;
using Back_End_Project.ViewModels.ProductVIewsModels;
using Back_End_Project.ViewModels.ShopViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Back_End_Project.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ShopController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? categoryId, int sort, int pageIndex = 1)
        {

            IQueryable<Product> productList = _context.Products.Where(p => !p.IsDeleted && (categoryId != null ? p.CategoryId ==categoryId : true)); //  Productlar

            if (sort == 1) // A-Z
            {
                productList = productList.OrderBy(p => p.Title);
            }
            else if (sort == 2) // Z-A
            {
                productList = productList.OrderByDescending(p => p.Title);
            }
            else if (sort == 3) // Bahalıdan ucuza
            {
                productList = productList.OrderByDescending(p => (p.DiscountedPrice > 0 ? p.DiscountedPrice : p.Price));
            }
            else if (sort == 4) // Ucuzdan bahaya
            {
                productList = productList.OrderBy(p => (p.DiscountedPrice > 0 ? p.DiscountedPrice : p.Price));
            }

            ShopVM vm = new()
            {
                CategoryId = categoryId,
                Sort = sort,
                Categories = await _context.Categories.Include(c => c.Products.Where(p => !p.IsDeleted )).Where(c => !c.IsDeleted).ToListAsync(),
                Products = PageNatedList<Product>.Create(productList, pageIndex, 12)
            };

            return View(vm);
        }
        
        public async Task<IActionResult> List(string? range = "")
        {
            double minValue = 0;
            double maxValue = 0;

            range = range?.Replace("$", "");

            if (!string.IsNullOrWhiteSpace(range))
            {
                string[] arr = range.Split(" - ");
                
                minValue = double.Parse(arr[0]);
                maxValue = double.Parse(arr[1]);
            }
            IEnumerable<Product> product = await _context.Products
                .Where(p => p.IsDeleted == false && (p.DiscountedPrice > 0  ? p.DiscountedPrice >= minValue && p.DiscountedPrice <= (maxValue == 0 ? 400 : maxValue)
                : p.Price >= minValue && p.Price <= (maxValue == 0 ? 400 : maxValue))).ToListAsync();

            return PartialView("_ShopListPartial", product);
        }
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => !pi.IsDeleted))
                .Include(p => p.Reviews.Where(r => !r.IsDeleted))
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (product == null) return NotFound();

            ProductReviewVM productReviewVM = new()
            {
                Product = product,
                Review = new Review {ProductId = id }
            };
            return View(productReviewVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddReview(Review review)
        {
            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(pi => !pi.IsDeleted && pi.ProductId == review.ProductId))
                .Include(p => p.Reviews.Where(r => !r.IsDeleted  ))
                .FirstOrDefaultAsync(p =>  p.Id == review.ProductId && !p.IsDeleted);


            ProductReviewVM productReviewVM = new ProductReviewVM
            {
                Product = product,
                Review = review
            };

            if (!ModelState.IsValid) return RedirectToAction("Detail",productReviewVM);
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            if (product.Reviews != null && product.Reviews.Count() > 0 && product.Reviews.Any(r => r.UserId == appUser.Id ) )
            {
                ModelState.AddModelError("Name", "Siz artiq FIkir Bildirmisiniz");
                return View("Detail", productReviewVM);
            }
            review.UserId = appUser.Id;
            review.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            review.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Detail),new { id = product.Id });
        }
        public async Task<IActionResult> Checkout()
        {
            return View();
        }
    }
}
