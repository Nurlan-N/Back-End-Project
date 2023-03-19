using Back_End_Project.Areas.Manage.ViewModels.UserVMs;
using Back_End_Project.DataAccessLayer;
using Back_End_Project.Models;
using Back_End_Project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Back_End_Project.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin")]

    public class OnlineController : Controller
    {
        private readonly AppDbContext _context;
        public OnlineController( AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            List<AppUser> onlineUsers = await _context.Users
                 .Where(u => u.LastOnline.HasValue && u.LastOnline.Value.AddMinutes(5) >= DateTime.UtcNow)
                 .ToListAsync();

            return View(PageNatedList<AppUser>.Create(onlineUsers.AsQueryable(), pageIndex, 3));
        }
    }
}
