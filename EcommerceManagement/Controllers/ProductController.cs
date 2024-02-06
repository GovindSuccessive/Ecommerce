using EcommerceManagement.Data;
using EcommerceManagement.Models.Domain;
using EcommerceManagement.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceManagement.Controllers
{
    
    public class ProductController : Controller
    {

        private readonly EcommerceDbContext _ecommerceDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly UserManager<UserModel> _userManager;

        public ProductController(EcommerceDbContext ecommerceDbContext, IWebHostEnvironment webHostEnvironment, SignInManager<UserModel> signInManager,
            UserManager<UserModel> userManager)
        {
            _ecommerceDbContext = ecommerceDbContext;
            _webHostEnvironment = webHostEnvironment;
            _signInManager = signInManager;
            _userManager = userManager;
        }

      
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            ViewBag.CategoryList = await _ecommerceDbContext.Categories.ToListAsync();
            return View();
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAllByCategory(Guid categoryId,string searchingQuery, string sortOrder)
        {
            var product =  await _ecommerceDbContext.Products.Include(x => x.Category).OrderByDescending(x=>x.ProductCreated).ToListAsync();
            if (categoryId != Guid.Empty)
            {
                if (categoryId != Guid.Parse("00000000-0000-0000-0000-000000000000"))
                {
                    product = product.Where(x => x.Category.CategoryId == categoryId).ToList();
                }
            }
            if (!string.IsNullOrEmpty(searchingQuery))
            {
                searchingQuery = searchingQuery.ToLower();
                product = product.Where(x => x.ProductName.ToLower().Contains(searchingQuery)).ToList();
            }

            switch (sortOrder)
            {
                case "normal":
                    product = product.ToList();
                    break;
                case "asc":
                    product = product.OrderBy(x => x.ProductPrice).ToList();
                    break;
                case "desc":
                    product = product.OrderByDescending(x => x.ProductPrice).ToList();
                    break;
            }
            return PartialView("_GetAllPartial",product);
        }

       
        [HttpGet]
        public async Task<IActionResult> GetProductsByCategory(Guid categoryId, string searchingQuery, string sortOrder)
        {
            var productList = await _ecommerceDbContext.Products
                .Include(x => x.Category)
                .Where(x => x.IsActive == true)
                .OrderByDescending(x => x.IsTrending)
                .Take(100)
                .ToListAsync();

            TempData["AdminSelectedCategoryId"] = categoryId;

            if (categoryId != Guid.Empty)
            {
                if (categoryId != Guid.Parse("00000000-0000-0000-0000-000000000000")){
                    productList = productList.Where(x => x.Category.CategoryId == categoryId).ToList();
                }
            }

            if (!string.IsNullOrEmpty(searchingQuery))
            {
                searchingQuery = searchingQuery.ToLower();
                productList = productList.Where(x => x.ProductName.ToLower().Contains(searchingQuery)).ToList();
            }

            switch (sortOrder)
            {
                case "normal":
                    productList = productList.ToList();
                    break;
                case "asc":
                    productList = productList.OrderBy(x => x.ProductPrice).ToList();
                    break;
                case "desc":
                    productList = productList.OrderByDescending(x => x.ProductPrice).ToList();
                    break;
            }

            return PartialView("_ProductListPartial", productList);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            ViewBag.CategoryList = await _ecommerceDbContext.Categories.ToListAsync();
            if (TempData.TryGetValue("AdminSelectedCategoryId", out var adminSelectedCategoryId))
            {
                ViewBag.DefaultCategoryId = (Guid)adminSelectedCategoryId;
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductDto addProductDto)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = "";
                if (addProductDto.ProductImage != null)
                {
                    string uploadFoler = Path.Combine(_webHostEnvironment.WebRootPath, "image");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + addProductDto.ProductImage.FileName;
                    string filePath = Path.Combine(uploadFoler, uniqueFileName);
                    addProductDto.ProductImage.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                var product = new ProductModel()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = addProductDto.ProductName,
                    ProductPrice = addProductDto.ProductPrice,
                    ProductDes = addProductDto.ProductDes!,
                    IsActive=true,
                    IsAvailable = addProductDto.IsAvailable,
                    IsTrending = addProductDto.IsTrending,
                    CategoryRefId = addProductDto.CategoryRefId,
                    ProductImage = uniqueFileName,
                    ProductCreated= DateTime.Now,
                };
                _ecommerceDbContext.Products.Add(product);
                await _ecommerceDbContext.SaveChangesAsync();
                TempData["productSuccess"] = "Product Added Successfully";
               
                return RedirectToAction("Index","Admin");
            }
            return View(addProductDto);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var product = await _ecommerceDbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            ViewBag.CategoryList = await _ecommerceDbContext.Categories.ToListAsync();
            if (product != null)
            {
                var newProduct = new ProductModel()
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductPrice = product.ProductPrice,
                    ProductDes = product.ProductDes,
                    IsAvailable = product.IsAvailable,
                    IsTrending = product.IsTrending,
                    ProductImage = product.ProductImage,
                    CategoryRefId = product.CategoryRefId,
                    Category = product.Category,
                };
                return await Task.Run(() => View("Update", newProduct));
            }
            return RedirectToAction("GetAll");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Update(ProductModel productModel)
        {
            var product = await _ecommerceDbContext.Products.FirstOrDefaultAsync(x => x.ProductId == productModel.ProductId);
            ViewBag.CourseList = _ecommerceDbContext.Categories.ToList();

            string uniqueFileName = "";
            if (productModel.ProductImageUplode != null)
            {
                string uploadFoler = Path.Combine(_webHostEnvironment.WebRootPath, "image");
                if (!string.IsNullOrEmpty(product.ProductImage))
                {
                    string previousImagePath = Path.Combine(uploadFoler, product.ProductImage);
                    if (System.IO.File.Exists(previousImagePath))
                    {
                        System.IO.File.Delete(previousImagePath);
                    }
                }
                uniqueFileName = Guid.NewGuid().ToString() + "_" + productModel.ProductImageUplode.FileName;
                string filePath = Path.Combine(uploadFoler, uniqueFileName);
                productModel.ProductImageUplode.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            if (product != null)
            {
                product.ProductName = productModel.ProductName;
                product.ProductPrice = productModel.ProductPrice;
                product.ProductDes = productModel.ProductDes;
                if (productModel.ProductImageUplode!=null)
                {
                    product.ProductImage = uniqueFileName;
                }
                product.IsAvailable= productModel.IsAvailable;
                product.IsTrending= productModel.IsTrending;
                product.CategoryRefId= productModel.CategoryRefId;
            }
                await _ecommerceDbContext.SaveChangesAsync();
            TempData["productSuccess"] = "Product Updated Successfully";
            return RedirectToAction("Index","Admin");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Active(Guid id)
        {
            var product = _ecommerceDbContext.Products.FirstOrDefault(x => x.ProductId == id);

            if (product != null)
            {
                product.IsActive = true;
                await _ecommerceDbContext.SaveChangesAsync();
            }
            return RedirectToAction("GetAll");               
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Deactive(Guid id)
        {
            var product =await _ecommerceDbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);

            if (product != null)
            {
                product.IsActive = false;
                await _ecommerceDbContext.SaveChangesAsync();
            }
            return RedirectToAction("GetAll");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = _ecommerceDbContext.Products.FirstOrDefault(x => x.ProductId == id);
            if (product != null)
            {
               _ecommerceDbContext.Products.Remove(product);
                await _ecommerceDbContext.SaveChangesAsync();
            }
            return RedirectToAction("GetAll");
        }

        


    }
}
