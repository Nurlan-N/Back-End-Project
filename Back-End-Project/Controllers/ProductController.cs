using Back_End_Project.DataAccessLayer;
using Back_End_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Back_End_Project.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> ProductModal(int? id)
        {
            Product product = await _context.Products .FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id );
             
            return PartialView("_ModalPartial",product);
        }
    }
}
