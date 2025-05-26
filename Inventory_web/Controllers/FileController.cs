using Inventoryweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventoryweb.Controllers
{
    public class FileController : Controller
    {
        private readonly InventoryDBContext _context;

        public FileController(InventoryDBContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Inventoryweb()
        {
            var product = await _context.Products.ToListAsync();
            return View("inventoryweb", product);
        }


        public IActionResult Addproduct()
        {


            return View();
        }

        [HttpPost]
        [Route("api/pro")]
        public IActionResult AddPro([FromForm] Products product)
        {
            if (product == null)
            {
                return BadRequest("Product data is missing.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Products.Add(product);
            _context.SaveChanges();

            return Ok("Product created successfully.");
        }

        [HttpGet]
        public async Task<IActionResult> Productpage()
        {
            var product = await _context.Products.ToListAsync();
            return View("Productpage", product);
        }
        [HttpGet]
        public async Task<IActionResult> LowStock()
        {
            var lowStockProducts = await _context.Products
                .Where(p => p.Stock < 5)
                .ToListAsync();
            return View("LowStock", lowStockProducts);
        }

        [HttpGet("product/details/{id}")]
        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return View("ProductNotFound", id);
            }
            return View("ProductDetails", product);
        }
        [HttpGet("product/delete/{id}")]
        public async Task<IActionResult> DeleteProductView(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                ViewBag.Message = $"Product with ID {id} not found.";
                ViewBag.IsSuccess = false;
                return View("DeleteResult");
            }

           
            product.IsDiscontinued = true;
            _context.Products.Update(product); 
            await _context.SaveChangesAsync();

            ViewBag.Message = $"Product with ID {id} marked as discontinued.";
            ViewBag.IsSuccess = true;
            return View("DeleteResult");
        }


        [HttpGet("product/edit/{id}")]
        public async Task<IActionResult> EditProductView(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return View("ProductNotFound", id);
            }
            return View("EditProductView", product);
        }

        [HttpPost("product/edit/{id}")]
        public IActionResult EditProductView(int id, Products model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingProduct = _context.Products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
            {
                return NotFound();
            }

          
            existingProduct.Name = model.Name;
            existingProduct.Description = model.Description;
            existingProduct.Category = model.Category;
            existingProduct.Price = model.Price;
            existingProduct.Stock = model.Stock;
            existingProduct.CreatedDate = model.CreatedDate;
            existingProduct.IsDiscontinued = model.IsDiscontinued;

            _context.SaveChanges();

            ViewBag.Message = "Product updated successfully!";
            return View(model);
        }


        public IActionResult Displaydata(string name, decimal? price, string category)
        {
            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(p => p.Name.Contains(name));
            }

            if (price.HasValue)
            {
                products = products.Where(p => p.Price == price.Value);
            }

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category);
            }

            var result = products.ToList();
            return View(result); 
        }




        public IActionResult ProductStats()
        {
            var totalProducts = _context.Products.Count();
            var lowStockCount = _context.Products.Count(p => p.Stock < 10);

            var topCategory = _context.Products
                .GroupBy(p => p.Category)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            var viewModel = new ProductStatisticsViewModel
            {
                TotalProducts = totalProducts,
                LowStockCount = lowStockCount,
                TopCategory = topCategory ?? "N/A"
            };

            return View(viewModel);
        }
      
        }



    







}
