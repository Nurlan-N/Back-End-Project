using Back_End_Project.DataAccessLayer;
using Back_End_Project.Interfaces;
using Back_End_Project.Models;
using Back_End_Project.ViewModels.BasketViewModels;
using Back_End_Project.ViewModels.WishlistViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Back_End_Project.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LayoutService(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<BasketVM>> GetBaskets()
        {
            string basket = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;
            if (!string.IsNullOrWhiteSpace(basket))
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

                foreach (BasketVM basketVM in basketVMs)
                {
                    Product product = await _appDbContext.Products
                        .FirstOrDefaultAsync(p => p.Id == basketVM.Id && p.IsDeleted == false);

                    if (product != null)
                    {
                        basketVM.ExTax = product.ExTax;
                        basketVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                        basketVM.Title = product.Title;
                        basketVM.Image = product.Image;
                    }

                }
            }
            else
            {
                basketVMs = new List<BasketVM>();
            }

            return basketVMs;
        }
        public async Task<IEnumerable<WishlistVM>> GetWishlist()
        {
            string wishlist = _httpContextAccessor.HttpContext.Request.Cookies["wishlist"];

            List<WishlistVM> wishlistVMs = null;
            if (!string.IsNullOrWhiteSpace(wishlist))
            {
                wishlistVMs = JsonConvert.DeserializeObject<List<WishlistVM>>(wishlist);

                foreach (WishlistVM wishlistVM in wishlistVMs)
                {
                    Product product = await _appDbContext.Products
                        .FirstOrDefaultAsync(p => p.Id == wishlistVM.Id && p.IsDeleted == false);

                    if (product != null)
                    {
                        wishlistVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                        wishlistVM.Title = product.Title;
                        wishlistVM.Image = product.Image;
                    }

                }
            }
            else
            {
                wishlistVMs = new List<WishlistVM>();
            }

            return wishlistVMs;
        }
        public async Task<IDictionary<string, string>> GetSettings()
        {
            IDictionary<string, string> settings = await _appDbContext.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);

            return settings;
        }

        
    }
}
