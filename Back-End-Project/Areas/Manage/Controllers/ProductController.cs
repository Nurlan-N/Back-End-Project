using Back_End_Project.DataAccessLayer;
using Back_End_Project.Models;
using Back_End_Project.Extentions;
using Back_End_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Back_End_Project.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(AppDbContext context, IWebHostEnvironment webHostEnvironment = null)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(int pageIndex = 1)
        {
            IQueryable<Product?> products = _context.Products.Where(p => p.IsDeleted == false);

            return View(PageNatedList<Product>.Create(products, pageIndex, 3));
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsDeleted == false).ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsDeleted == false ).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(b => b.IsDeleted == false).ToListAsync();
            if (!ModelState.IsValid) return View(product);

            if (!await _context.Categories.AnyAsync(c => c.IsDeleted == false && c.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", $"Daxil olunan Bran Id {product.CategoryId} Yalnisdir");
                return View(product);
            }


            if (product.ImageFile != null)
            {
                if (!product.ImageFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("ImageFile", "ImageFile File Yalniz JPG Formatda ola biler");
                    return View(product);
                }
                if (!product.ImageFile.CheckFileLength(300))
                {
                    ModelState.AddModelError("ImageFile", "ImageFile File Yalniz 300Kb  ola biler");
                    return View(product);
                }
                product.Image = await product.ImageFile.CreateFileAsync(_webHostEnvironment, "assets", "img");
            }
            else
            {
                ModelState.AddModelError("ImageFile", "ImageFile File mutleqdir");
                return View(product);
            }
          

            if (product.Files.Count() <= 6)
            {
                if (product.Files != null && product.Files.Count() > 0)
                {
                    List<ProductImage> productImages = new List<ProductImage>();
                    foreach (IFormFile file in product.Files)
                    {
                        if (!file.CheckFileContentType("image/jpeg"))
                        {
                            ModelState.AddModelError("file", "Main File Yalniz JPG Formatda ola biler");
                            return View(product);
                        }
                        if (!file.CheckFileLength(300))
                        {
                            ModelState.AddModelError("file", "Main File Yalniz 300Kb  ola biler");
                            return View(product);
                        }
                        ProductImage productImage = new ProductImage()
                        {
                            Image = await file.CreateFileAsync(_webHostEnvironment, "assets", "img", "product"),
                            CreatedAt = DateTime.UtcNow.AddDays(4),
                            CreatedBy = "System"
                        };
                    }

                    product.ProductImages = productImages;
                }
            }
            else
            {
                ModelState.AddModelError("Files", "max 6 shekil");
                return View(product);
            }

            string code = product.Title.Substring(0, 2);
            code = code + _context.Categories.FirstOrDefault(c => c.Id == product.CategoryId).Name.Substring(0, 1);
            product.Seria = code.ToLower().Trim();
            product.Code = _context.Products.Where(p => p.Seria == product.Seria)
                .OrderByDescending(p => p.Id).FirstOrDefault() != null ?
                _context.Products.Where(p => p.Seria == product.Seria).OrderByDescending(p => p.Id).FirstOrDefault().Code += 1 : 1;

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
