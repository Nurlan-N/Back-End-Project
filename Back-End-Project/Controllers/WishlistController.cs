﻿using Back_End_Project.DataAccessLayer;
using Back_End_Project.Models;
using Back_End_Project.ViewModels.BasketViewModels;
using Back_End_Project.ViewModels.WishlistViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Back_End_Project.Controllers
{
    public class WishlistController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public WishlistController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> AddWishlist(int? id)
        {
            if (id == null) { return BadRequest(); }

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id))
            {
                return NotFound();
            }

            string wishlist = HttpContext.Request.Cookies["wishlist"];

            List<WishlistVM> wishlistVMs = null;


            if (string.IsNullOrWhiteSpace(wishlist))
            {
                wishlistVMs = new List<WishlistVM>
                {
                    new WishlistVM { Id = (int)id, Count = 1 }
                };


            }
            else
            {
                wishlistVMs = JsonConvert.DeserializeObject<List<WishlistVM>>(wishlist);

                if (wishlistVMs.Exists(b => b.Id == id))
                {
                    wishlistVMs.Find(b => b.Id == id).Count += 1;
                }
                else
                {
                    wishlistVMs.Add(new WishlistVM { Id = (int)id, Count = 1 });
                }

            }
            if (User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.Users.Include(u => u.Wishlist.Where(b => b.IsDeleted == false)).FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

                if (appUser.Wishlist.Any(b => b.ProductId == id))
                {
                    appUser.Wishlist.FirstOrDefault(b => b.ProductId == id).Count = wishlistVMs.FirstOrDefault(b => b.Id == id).Count;
                }
                else
                {
                    Wishlist dbWishlist = new()
                    {
                        ProductId = id,
                        Count = wishlistVMs.FirstOrDefault(b => b.Id == id).Count,
                    };

                    appUser.Wishlist.Add(dbWishlist);
                    await _context.SaveChangesAsync();

                }


            }

            wishlist = JsonConvert.SerializeObject(wishlistVMs);

            HttpContext.Response.Cookies.Append("wishlist", wishlist);

            foreach (WishlistVM wishlistVM in wishlistVMs)
            {
                Product product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == wishlistVM.Id && p.IsDeleted == false);

                if (product != null)
                {
                    wishlistVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                    wishlistVM.Title = product.Title;
                    wishlistVM.Image = product.Image;
                }

            }

            return PartialView("_WishlistPartial", wishlistVMs);

        }

        public async Task<IActionResult> DeleteWishlist(int? id)
        {
            if (id == null) { return BadRequest(); }

            if (!await _context.Products.AnyAsync(p => p.IsDeleted == false && p.Id == id))
            {
                return NotFound();
            }
            string wishlist = HttpContext.Request.Cookies["wishlist"];

            List<WishlistVM> wishlistVMs = null;
            if (string.IsNullOrWhiteSpace(wishlist))
            {
                return BadRequest();
            }
            else
            {
                wishlistVMs = JsonConvert.DeserializeObject<List<WishlistVM>>(wishlist);
                if (wishlistVMs.Exists(b => b.Id == id))
                {
                    WishlistVM newWishlist = wishlistVMs.Find(b => b.Id == id);
                    wishlistVMs.Remove(newWishlist);
                    wishlist = JsonConvert.SerializeObject(wishlistVMs);
                    HttpContext.Response.Cookies.Append("wishlist", wishlist);
                }
                else
                {
                    return NotFound();
                }
                foreach (WishlistVM wishlistVM in wishlistVMs)
                {
                    Product product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == wishlistVM.Id && p.IsDeleted == false);

                    if (product != null)
                    {
                        wishlistVM.Price = product.DiscountedPrice > 0 ? product.DiscountedPrice : product.Price;
                        wishlistVM.Title = product.Title;
                        wishlistVM.Image = product.Image;
                    }

                }


                return PartialView("_WishlistPartial", wishlistVMs);

            }
        }

        public IActionResult GetBasket()
        {

            return Json(JsonConvert.DeserializeObject<List<BasketVM>>(HttpContext.Request.Cookies["basket"]));
        }

    }
}
