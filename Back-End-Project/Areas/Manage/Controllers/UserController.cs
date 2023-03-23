using Back_End_Project.Areas.Manage.ViewModels.AccountVMs;
using Back_End_Project.Areas.Manage.ViewModels.UserVMs;
using Back_End_Project.DataAccessLayer;
using Back_End_Project.Models;
using Back_End_Project.ViewModels;
using Back_End_Project.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using RegisterVM = Back_End_Project.Areas.Manage.ViewModels.AccountVMs.RegisterVM;

namespace Back_End_Project.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<AppUser> userManager, AppDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            List<UserVM> query = await _userManager.Users.Where(u => u.UserName != User.Identity.Name)
                .Select(x => new UserVM
                {
                    Id = x.Id,
                    Name = x.Name,
                    SurName = x.SurName,
                    Email = x.Email,
                    UserName = x.UserName,
                    LockoutEnd = (DateTimeOffset)x.LockoutEnd
                })
            .ToListAsync();

            foreach (var item in query)
            {
                var userRole = _context.UserRoles.FirstOrDefault(u => u.UserId == item.Id);
                string roleId = userRole != null ? userRole.RoleId : null;
                var role = _context.Roles.FirstOrDefault(r => r.Id == roleId);
                string roleName = role != null ? role.Name : null;

                item.RoleName = roleName;
            }
            return View(PageNatedList<UserVM>.Create(query.AsQueryable(), pageIndex, 5));
        }
        [HttpGet]
        public async Task<IActionResult> Create( )
        {
            ViewBag.Role = await _roleManager.Roles.Where(c => c.Name != "SuperAdmin").ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RegisterVM registerVM)
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
            string roleId = _roleManager.Roles.FirstOrDefault(c => c.Name != "SuperAdmin" && c.Id == registerVM.RoleId).Name;

            await _userManager.AddToRoleAsync(appUser, roleId);

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> ChangeRole(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            AppUser appUser = await _userManager.FindByIdAsync(id);

            if (appUser == null) { return NotFound(); }

            string roleId = _context.UserRoles.FirstOrDefault(u => u.UserId == appUser.Id).RoleId;

            UserChangeRoleVM userChangeRoleVM = new UserChangeRoleVM
            {
                UserId = appUser.Id,
                RoleId = roleId,
            };

            ViewBag.Role = await _roleManager.Roles.Where(c => c.Name != "SuperAdmin").ToListAsync();

            return View(userChangeRoleVM);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeRole(UserChangeRoleVM userChangeRoleVM)
        {
            ViewBag.Role = await _roleManager.Roles.Where(c => c.Name != "SuperAdmin").ToListAsync();

            if (!ModelState.IsValid) { return View(userChangeRoleVM); }


            AppUser appUser = await _userManager.FindByIdAsync(userChangeRoleVM.UserId);

            if (appUser == null) { return NotFound(); }

            string roleId = _context.UserRoles.FirstOrDefault(u => u.UserId == userChangeRoleVM.UserId).RoleId;
            string roleName = _context.Roles.FirstOrDefault(r => r.Id == roleId).Name;
            string newRoleName = _roleManager.Roles.FirstOrDefault(c => c.Name != "SuperAdmin" && c.Id == userChangeRoleVM.RoleId).Name;

            await _userManager.RemoveFromRoleAsync(appUser, roleName);
            await _userManager.AddToRoleAsync(appUser, newRoleName);

            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Block(string? id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);

            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Block(AppUser appUser )
        {
            if (!ModelState.IsValid) { return View(appUser); }

            AppUser user = await _userManager.FindByIdAsync(appUser.Id);
            DateTimeOffset? lockoutEnd = appUser.LockoutEnd;
            await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);

            if (user == null) { return NotFound(); }


            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Unblock(string userId)
        {
            if (string.IsNullOrEmpty(userId)) { return NotFound(); }

            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user == null) { return NotFound(); }

            await _userManager.SetLockoutEndDateAsync(user, null);

            return RedirectToAction("Index");
        }
    }
}
