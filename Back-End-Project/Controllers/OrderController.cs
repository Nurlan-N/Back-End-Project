using Back_End_Project.DataAccessLayer;
using Back_End_Project.Models;
using Back_End_Project.ViewModels.BasketViewModels;
using Back_End_Project.ViewModels.OrderVIewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Back_End_Project.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;

        public OrderController(UserManager<AppUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            string coockie = HttpContext.Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(coockie)) return RedirectToAction("Index", "Shop");

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(coockie);

            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);
                basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                basketVM.Title = product.Title;

            }

            AppUser appUser = await _userManager.Users
                .Include(u => u.Addresses.Where(a => a.IsMain && !a.IsDeleted))
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            Order order = new()
            {
                Name = appUser.Name,
                SurName = appUser.SurName,
                Email = appUser.Email,
                Phone = appUser.Addresses.FirstOrDefault().Phone,
                City = appUser.Addresses.FirstOrDefault().City,
                Country = appUser.Addresses.FirstOrDefault().Country,
                State = appUser.Addresses.FirstOrDefault().State,
                PostalCode = appUser.Addresses.FirstOrDefault().PostalCode
            };

            OrderVM orderVM = new()
            {
                Order = order,
                BasketVMs = basketVMs,
            };

            return View(orderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Orders)
                .Include(u => u.Addresses.Where(a => a.IsMain && !a.IsDeleted))
                .Include(u => u.Baskets.Where(ub => !ub.IsDeleted))
                .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            string coockie = HttpContext.Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(coockie)) return RedirectToAction("Index", "Shop");

            List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(coockie);

            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.Id);
                basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                basketVM.Title = product.Title;
            }

            OrderVM orderVM = new()
            {
                Order = order,
                BasketVMs = basketVMs,
            };

            if (!ModelState.IsValid) return View(orderVM);

            List<OrderItem> orderItems = basketVMs.Select(basketVM => new OrderItem
            {
                Count = basketVM.Count,
                ProductId = basketVM.Id,
                Price = basketVM.Price,
                CreatedAt = DateTime.UtcNow.AddHours(4),
                CreatedBy = $"{appUser.Name} {appUser.SurName}"
            }).ToList();
           

            foreach (Basket basket in appUser.Baskets)
            {
                basket.IsDeleted = true;
            }

            HttpContext.Response.Cookies.Append("basket", "");

            order.UserId = appUser.Id;
            order.CreatedAt = DateTime.UtcNow.AddHours(4);
            order.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            order.OrderItems = orderItems;
            order.No = (appUser.Orders?.Count() ?? 0) > 0 ? appUser.Orders.Last().No + 1 : 1;

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();



            return RedirectToAction("index", "home");
        }
    }
}
