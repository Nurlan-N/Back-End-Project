using Back_End_Project.DataAccessLayer;
using Back_End_Project.Models;
using Back_End_Project.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using Back_End_Project.ViewModels.BasketViewModels;
using Newtonsoft.Json;
using Back_End_Project.ViewModels.WishlistViewModels;

namespace Back_End_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, AppDbContext context,
            RoleManager<IdentityRole> roleManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) { return View(); }
            AppUser appUser = new AppUser
            {
                Name = registerVM.Name,
                Email = registerVM.Email,
                SurName = registerVM.SurName,
                UserName = registerVM.UserName,
            };
            IdentityResult identityResult = await _userManager.CreateAsync(appUser, registerVM.Password);
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError identityError in identityResult.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
                return View(registerVM);
            }
            await _userManager.AddToRoleAsync(appUser, "Menber");

            return RedirectToAction("Index");

        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) { return View(loginVM); }

            AppUser appUser = await _userManager.Users.Include(u => u.Baskets.Where(b => b.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedEmail == loginVM.Email.Trim().ToUpperInvariant());

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email veya Password Yalnisdir");
                return View(loginVM);
            }
            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(appUser, loginVM.Password, loginVM.RememberMe, true);
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("", "Siz Blok olunmusuz");
                return View(loginVM);
            }
            if (!signInResult.Succeeded)
            {
                ModelState.AddModelError("", "Email veya Password Yalnisdir");
                return View(loginVM);
            }

            string basket = HttpContext.Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(basket))
            {
                if (appUser.Baskets != null && appUser.Baskets.Count() > 0)
                {
                    List<BasketVM> basketVMs = new List<BasketVM>();

                    foreach (Basket basket1 in appUser.Baskets)
                    {
                        BasketVM basketVM = new()
                        {
                            Id = (int)basket1.ProductId,
                            Count = basket1.Count,
                        };
                        basketVMs.Add(basketVM);
                    }

                    basket = JsonConvert.SerializeObject(basketVMs);

                    HttpContext.Response.Cookies.Append("basket", basket);
                }
            }
            else
            {
                HttpContext.Response.Cookies.Append("basket", "");

            }
            string wishlist = HttpContext.Request.Cookies["wishlist"];

            if (string.IsNullOrWhiteSpace(wishlist))
            {
                if (appUser.Wishlist != null && appUser.Wishlist.Count() > 0)
                {
                    List<WishlistVM> wishlistVMs = new List<WishlistVM>();

                    foreach (Wishlist wishlist1 in appUser.Wishlist)
                    {
                        WishlistVM wishlistVM = new()
                        {
                            Id = (int)wishlist1.ProductId,
                            Count = wishlist1.Count,
                        };
                        wishlistVMs.Add(wishlistVM);
                    }

                    wishlist = JsonConvert.SerializeObject(wishlistVMs);

                    HttpContext.Response.Cookies.Append("wishlist", wishlist);
                }
            }
            else
            {
                HttpContext.Response.Cookies.Append("basket", "");

            }

            appUser.LastOnline = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();
            return RedirectToAction("index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("index", "Home");
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyAccount(Address? address)
        {
            AppUser appUser = await _userManager.Users
                .Include(u => u.Orders.Where(uo => !uo.IsDeleted))
                .ThenInclude(uo => uo.OrderItems.Where(oi => !oi.IsDeleted))
                .ThenInclude(oi => oi.Product)
                .Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name
                .ToUpperInvariant());
            ProfileVM profileVM = new()
            {
                Name = appUser.Name,
                SurName = appUser.SurName,
                Email = appUser.Email,
                UserName = appUser.UserName,
                Addresses = appUser.Addresses,
                Orders = appUser.Orders,
                EditAddress = address

            };
            return View(profileVM);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyAccount(ProfileVM profileVM)
        {
            if (!ModelState.IsValid) return View(profileVM);

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            appUser.Name = profileVM.Name;
            appUser.SurName = profileVM.SurName;
            if (appUser.NormalizedEmail != profileVM.Email.Trim().ToUpperInvariant()) { appUser.Email = profileVM.Email; }
            if (appUser.NormalizedUserName != profileVM.UserName.Trim().ToUpperInvariant()) { appUser.UserName = profileVM.UserName; }

            IdentityResult result = await _userManager.UpdateAsync(appUser);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(profileVM);

            }
            await _signInManager.SignInAsync(appUser, true);

            if (!string.IsNullOrWhiteSpace(profileVM.OldPassword))
            {
                if (await _userManager.CheckPasswordAsync(appUser, profileVM.OldPassword))
                {
                    ModelState.AddModelError("OldPassword", "Old Password Yalnishdi");
                    TempData["Tab"] = "account";
                    return View(profileVM);
                }
                if (profileVM.OldPassword == profileVM.Password)
                {
                    ModelState.AddModelError("Password", "Yeni Şifrə Köhnə ilə eyni ola bilməz");
                    TempData["Tab"] = "account";
                    return View(profileVM);
                }

                string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
                result = await _userManager.ResetPasswordAsync(appUser, token, profileVM.Password);
                if (!result.Succeeded)
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);

                    }
                    TempData["Tab"] = "account";
                    return View(profileVM);

                }
            }

            return Redirect("/");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress(Address address)
        {
            AppUser appUser = await _userManager.Users.Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            ProfileVM profileVM = new ProfileVM
            {
                Addresses = appUser.Addresses
            };

            if (!ModelState.IsValid) return View(nameof(MyAccount), profileVM);
            if (address.IsMain && appUser.Addresses != null && appUser.Addresses.Count() > 0 && appUser.Addresses.Any(u => u.IsMain))
            {
                appUser.Addresses.FirstOrDefault(a => a.IsMain).IsMain = false;
            }
            address.UserId = appUser.Id;
            address.CreatedBy = $"{appUser.Name} {appUser.SurName}";
            address.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();

            TempData["Tab"] = "address";

            return RedirectToAction(nameof(MyAccount));
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditAddress(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            AppUser appUser = await _userManager.Users.Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                 .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            Address oldAddress = appUser.Addresses.FirstOrDefault(a => a.Id == id && !a.IsDeleted);
            if (oldAddress == null)
            {
                return NotFound();
            }

            Address address = new Address
            {
                City = oldAddress.City,
                Country = oldAddress.Country,
                PostalCode = oldAddress.PostalCode,
                Phone = oldAddress.Phone,
                State = oldAddress.State,
                IsMain = oldAddress.IsMain,
                Id = oldAddress.Id
            };

            TempData["Tab"] = "address";

            return RedirectToAction(nameof(MyAccount),address);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(Address address)
        {
            if (address == null || address.Id == null){return NotFound();}

            AppUser user = await _userManager.Users.Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                 .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());

            if (user == null) {return NotFound();}

            Address oldAddress = user.Addresses.FirstOrDefault(a => a.Id == address.Id && !a.IsDeleted);
            if (oldAddress == null)
            {
                return NotFound();
            }

            bool isMain = address.IsMain;
            if (isMain)
            {
                IEnumerable<Address> otherAddresses = user.Addresses.Where(a => a.Id != address.Id && !a.IsDeleted && a.IsMain);
                foreach (Address otherAddress in otherAddresses)
                {
                    otherAddress.IsMain = false;
                }
            }

            oldAddress.City = address.City;
            oldAddress.Country = address.Country;
            oldAddress.PostalCode = address.PostalCode;
            oldAddress.Phone = address.Phone;
            oldAddress.State = address.State;
            oldAddress.IsMain = isMain;

            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();


            TempData["Tab"] = "address";
            TempData["Message"] = "Address updated successfully";

            return RedirectToAction(nameof(MyAccount));
        }

    }
}
