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

            AppUser appUser = await _userManager.FindByEmailAsync(loginVM.Email);
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
        public async Task<IActionResult> MyAccount()
        {
            AppUser appUser = await _userManager.Users.Include(u => u.Addresses.Where(a => a.IsDeleted == false))
                .FirstOrDefaultAsync(u => u.NormalizedUserName == User.Identity.Name.ToUpperInvariant());
            ProfileVM profileVM = new()
            {
                Name = appUser.Name,
                SurName = appUser.SurName,
                Email = appUser.Email,
                UserName = appUser.UserName,
                Addresses = appUser.Addresses

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
            if (appUser.NormalizedEmail != profileVM.Email.Trim().ToUpperInvariant()) {appUser.Email = profileVM.Email; }
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

            if(!string.IsNullOrWhiteSpace(profileVM.OldPassword)) 
            {
                if(await _userManager.CheckPasswordAsync(appUser, profileVM.OldPassword)) 
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
                result =  await _userManager.ResetPasswordAsync(appUser,token,profileVM.Password);
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
    }
}
