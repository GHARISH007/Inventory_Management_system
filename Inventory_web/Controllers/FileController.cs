using Inventoryweb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Inventoryweb.Controllers // project name indiating that it was controller
{
    public class FileController : Controller   //  inherited with  base class contoller 
    {
        private readonly InventoryDBContext _context; // declaring  a variable for only reading  

        public FileController(InventoryDBContext context) // passing db context paramater to    variable  context in constructor
        {
            _context = context;
        }
        public async Task<IActionResult> Inventoryweb()     // creating function  inventoryweb in async  also displaying action result
        {
            var product = await _context.Products.ToListAsync();   //converting all the data in table and proccesing them to list and storing them in table
            return View("inventoryweb", product); // returning product value to inventoryweb view
        }


        public IActionResult Addproduct() // creating function  and returning http result 
        {


            return View();      //returning appproduct view            
        }
        [HttpPost]
        [Route("api/add_product")]   //route for add_product api
        public async Task<IActionResult> AddPro([FromForm] Products product) //  passsing form input  in parameters for product
        {
            if (product == null)                                         // if product data is null
                return BadRequest("Product data is missing.");          // data is missing

            if (!ModelState.IsValid)                                          // if model  state  all values are is not valid
                return BadRequest(ModelState);            // return badrequest

            bool nameExists = await _context.Products        // check for tha name field if any aready exixts in db
    .AnyAsync(p => p.Name.ToLower() == product.Name.ToLower()); // and matches the input name   field and db name fields

            if (nameExists)
                return BadRequest("Product name is already there."); // if exist there  it shows there is  already a  name
            // ✅ Handle image upload
            if (product.ImageFile != null && product.ImageFile.Length > 0) // if image file is null and image file is greater then 0 lenght
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images"); // then stores image in root image folder
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(product.ImageFile.FileName); // gendrating a unique id for each image 
                var filePath = Path.Combine(uploadsFolder, uniqueFileName); // assigning  a unique file name to uplorded folder

                using (var stream = new FileStream(filePath, FileMode.Create)) // assigning  a unique file name to uplorded folder
                {
                    await product.ImageFile.CopyToAsync(stream);// it takes a  uplorded file a copy and stores in the file stream
                }

                product.ImagePath = "/images/" + uniqueFileName; // at  this path it store the image file
            }

            // ✅ Save product with image path
            _context.Products.Add(product); // makes  db context to assingn this produt value to product table 
            await _context.SaveChangesAsync(); // and save the changes

            return Ok("Product created successfully."); // returns ok message
        }



        [HttpGet]
        public async Task<IActionResult> Productpage()    // creating a productpage function
        {
            var product = await _context.Products.ToListAsync();  // making list and storeing the data in product  variable
            return View("Productpage", product);// and returing the product variable in productpage view 
        }
        [HttpGet]
        public async Task<IActionResult> LowStock() // creating lostock fucntion
        {
            var lowStockProducts = await _context.Products//  geting products list for stock less then 5
                .Where(p => p.Stock < 5)                   // and searching for the product whaich has low stock less then 5
                .ToListAsync();                                // and converting them to list 
            return View("LowStock", lowStockProducts);       // returnig the values to the lowstock view      
        }

        [HttpGet("product/details/{id}")]
        public async Task<IActionResult> ProductDetails(int id) // creating a productdetails
        {
            var product = await _context.Products.FindAsync(id);// 
            if (product == null)
            {
                return View("ProductNotFound", id);
            }
            return View("ProductDetails", product);
        }
        [HttpGet("product/delete/{id}")]
        public async Task<IActionResult> DeleteProductView(int id)  // get inp id of the  selected row
        {
            var product = await _context.Products.FindAsync(id); // finds product relateated  to id ans assign them to product variable  
            if (product == null) // if it is null 
            {
                ViewBag.Message = $"Product with ID {id} not found.";  // retund product not found
                ViewBag.IsSuccess = false; // and statsu becomes false
                return View("DeleteResult"); // retuns delete result  in delete result
            }


            product.IsDiscontinued = true;   // if its is not null then make them  true 
            _context.Products.Update(product);  // and update products list
            await _context.SaveChangesAsync(); // and saves the changes

            ViewBag.Message = $"Product with ID {id} marked as discontinued.";// displays the product id is marked as discontinous 
            ViewBag.IsSuccess = true;// and shows discontious status to true
            return View("DeleteResult");// returns deleteresult view
        }


        [HttpGet("product/edit/{id}")]
        public async Task<IActionResult> EditProductView(int id) // creating editproduct function
        {
            var product = await _context.Products.FindAsync(id);  // get relevant product details on id input and stores at product
            if (product == null) // if product is null
            {
                return View("ProductNotFound", id);//return product not found
            }
            return View("EditProductView", product); // or return new page 
        }

        [HttpPost("product/edit/{id}")]
        public async Task<IActionResult> EditProductView(int id, Products model, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate ImagePath if the model is invalid and needs to return to view
                var existing = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                if (existing != null)
                    model.ImagePath = existing.ImagePath;

                return View(model);
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
                return NotFound();

            bool nameExists = _context.Products.Any(p => p.Id != id && p.Name.ToLower() == model.Name.ToLower());
            if (nameExists)
            {
                model.ImagePath = existingProduct.ImagePath; // retain image path
                ModelState.AddModelError("Name", "Product name is already there.");
                return View(model);
            }

            // Update values
            existingProduct.Name = model.Name;
            existingProduct.Description = model.Description;
            existingProduct.Category = model.Category;
            existingProduct.Price = model.Price;
            existingProduct.Stock = model.Stock;
            existingProduct.CreatedDate = model.CreatedDate;
            existingProduct.IsDiscontinued = model.IsDiscontinued;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                existingProduct.ImagePath = "/images/" + fileName;
            }

            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();

            ViewBag.Message = "✅ Product updated successfully!";
            return View("EditProductView", existingProduct);
        }



        public IActionResult Displaydata(string name, decimal? price, string category)  // get name ,price , and cataory as input
        {
            var products = _context.Products.AsQueryable(); // converts product view into  as queue

            if (!string.IsNullOrEmpty(name))      //if name  string is not null and not empty   
            {
                products = products.Where(p => p.Name.Contains(name));// then searches the product name in db
            }

            if (price.HasValue) // if price has value
            {
                products = products.Where(p => p.Price == price.Value); // searaches for price
            }

            if (!string.IsNullOrEmpty(category)) // idf catagory is not null or not empty
            {
                products = products.Where(p => p.Category == category); // seraches the product catagories in db
            }

            var result = products.ToList(); // if any matches then return to result
            return View(result); // return result
        }




        public IActionResult ProductStats() // creating a function
        {
            var totalProducts = _context.Products.Count(); // taking count on total product 
            var lowStockCount = _context.Products.Count(p => p.Stock < 10);// taking count on low stock

            var topCategory = _context.Products
                .GroupBy(p => p.Category)            // group by to catagories
                .OrderByDescending(g => g.Count()) // make them descending order 
                .Select(g => g.Key) 
                .FirstOrDefault();

            var viewModel = new ProductStatisticsViewModel // store in view maodel variable 
            {
                TotalProducts = totalProducts, // haves the total count 
                LowStockCount = lowStockCount,// haves the total low stck count
                TopCategory = topCategory ?? "N/A" // have   the top catagories values
            };

            return View(viewModel); //r etuns view model
        }

    }










}
