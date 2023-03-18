using Back_End_Project.DataAccessLayer;
using Back_End_Project.Models;
using Back_End_Project.Extentions;
using Back_End_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Back_End_Project.Helpers;

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
                .Where(c => c.IsDeleted == false).ToListAsync();
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
                product.Image = await product.ImageFile.CreateFileAsync(_webHostEnvironment, "assets", "img", "product");
            }
            else
            {
                ModelState.AddModelError("ImageFile", "ImageFile File mutleqdir");
                return View(product);
            }


            if (product?.Files?.Count() <= 6)
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
                        productImages.Add(productImage);
                    }

                    product.ProductImages = productImages;
                    product.CreatedAt = DateTime.UtcNow.AddDays(4);
                    product.CreatedBy = "System";
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
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(t => t.IsDeleted == false))
                .FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
            if (product == null) return NotFound();

            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(b => b.IsDeleted == false).ToListAsync();


            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id, Product product)
        {
            ViewBag.Categories = await _context.Categories
                .Where(c => c.IsDeleted == false).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(b => b.IsDeleted == false).ToListAsync();

            if (!ModelState.IsValid) return View();

            if (id == null || id != product.Id) return BadRequest();

            Product dbProduct = await _context.Products
                .Include(p => p.ProductImages.Where(pImages => pImages.IsDeleted == false))
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);

            if (dbProduct == null) return NotFound();

            int canUpload = 6 - dbProduct.ProductImages.Count();
            if (product.Files != null && canUpload < product.Files.Count())
            {
                ModelState.AddModelError("Files", $"Maksimum {canUpload} Qeder sekil yukleye bilersiniz");
                return View(product);

            }
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
                    productImages.Add(productImage);
                }

                dbProduct.ProductImages.AddRange(productImages);
            }
            //StartImageFile
            if (product.ImageFile != null)
            {
                if (!product.ImageFile.CheckFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", "Main File Yalniz JPG Formatda ola biler");
                    return View(product);
                }
                if (!product.ImageFile.CheckFileLength(300))
                {
                    ModelState.AddModelError("MainFile", "Main File Yalniz 300Kb  ola biler");
                    return View(product);
                }
                FileHelpers.DeleteFile(dbProduct.Image, _webHostEnvironment, "assets", "img", "product");

                dbProduct.Image = await product.ImageFile.CreateFileAsync(_webHostEnvironment, "assets", "img", "product");
            }
            if (product.Price != null) { dbProduct.Price = product.Price; }
            if (product.DiscountedPrice != null) { dbProduct.DiscountedPrice = product.DiscountedPrice; }
            if (product.Count != null) { dbProduct.Count = product.Count; }
            if (product.ExTax != null) { dbProduct.ExTax = product.ExTax; }
            if (product.Description != null) { dbProduct.Description = product.Description; }

            await _context.SaveChangesAsync();



            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteImage(int id, int imageId)
        {
            if (id == null) return BadRequest();

            if (imageId == null) return BadRequest();

            Product product = await _context.Products
                .Include(p => p.ProductImages.Where(p => p.IsDeleted == false))
                .FirstOrDefaultAsync(P => P.IsDeleted == false && P.Id == id);

            if (product == null) return NotFound();

            if (product.ProductImages.Any(p => p.Id == imageId))
            {
                product.ProductImages.FirstOrDefault(product => product.Id == imageId).IsDeleted = true;
                await _context.SaveChangesAsync();

                FileHelpers.DeleteFile(product.ProductImages.FirstOrDefault(product => product.Id == imageId).Image, _webHostEnvironment, "assets", "img", "product");

            }
            else
            {
                return BadRequest();
            }
            List<ProductImage> productImages = product.ProductImages.Where(p => p.IsDeleted == false).ToList();



            return PartialView("_ProductImagePartial", productImages);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id, int pageIndex = 1)
        {
            if (id == null) { return BadRequest(); }

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);

            if (product == null) { return NotFound(); }

            product.IsDeleted = true;
            product.DeletedBy = "System";
            product.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            IQueryable<Product> products = _context.Products.Where(b => b.IsDeleted == false).OrderByDescending(b => b.Id);


            return View("index", PageNatedList<Product>.Create(products, pageIndex, 3));
        }
    }
}
