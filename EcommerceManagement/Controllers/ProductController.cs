using EcommerceManagement.Data;
using EcommerceManagement.Models.Domain;
using EcommerceManagement.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceManagement.Controllers
{
    public class ProductController : Controller
    {

        private readonly EcommerceDbContext _ecommerceDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(EcommerceDbContext ecommerceDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _ecommerceDbContext = ecommerceDbContext;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            /*var productList = await _ecommerceDbContext.Products.ToListAsync();
            var categoryList =await  _ecommerceDbContext.Categories.ToListAsync();
            var categoryProduct = productList.Join(// outer sequence 
                       categoryList,  // inner sequence 
                       product => product.CategoryRefId,   // outerKeySelector
                       category => category.CategoryId, // innerKeySelector
                       (product, category) => new ProductCategoryDto // result selector
                       {
                           ProductId = product.ProductId,
                           ProductName = product.ProductName,
                           ProductDes = product.ProductDes,
                           ProductPrice = product.ProductPrice,
                           ProductImage = product.ProductImage,
                           IsAvailable = product.IsAvailable,
                           IsActive = product.IsActive,
                           ProductCreated=product.ProductCreated,
                           IsTrending = product.IsTrending,
                           CategoryId = category.CategoryId,
                           CategoryName = category.CategoryName
                       }).OrderByDescending(x=>x.ProductCreated).ToList();*/
            ViewBag.CategoryList = await _ecommerceDbContext.Categories.ToListAsync();
            return View();
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetAllByCategory(Guid categoryId)
        {
            var product =  await _ecommerceDbContext.Products.Include(x => x.Category).ToListAsync();
            if(categoryId== Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                return PartialView("_GetAllPartial",product);
            }
            return PartialView("_GetAllPartial",product.Where(x => x.CategoryRefId == categoryId).ToList());
        }

        [HttpGet]

        public async Task<IActionResult> GetProductsByCategory(Guid categoryId)
        {
            var productList = await _ecommerceDbContext.Products.Include(x => x.Category).Where(x=>x.IsActive == true).Take(100).ToListAsync();
            /* var categoryList = await _ecommerceDbContext.Categories.ToListAsync();
             var categoryProduct =  productList.Join(// outer sequence 
                        categoryList,  // inner sequence 
                        product => product.CategoryRefId,   // outerKeySelector
                        category => category.CategoryId, // innerKeySelector
                        (product, category) => new ProductCategoryDto // result selector
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductName,
                            ProductDes = product.ProductDes,
                            ProductPrice = product.ProductPrice,
                            ProductImage = product.ProductImage,
                            IsAvailable = product.IsAvailable,
                            IsTrending = product.IsTrending,
                            CategoryId = category.CategoryId,
                            CategoryName = category.CategoryName

                        }).Where(x=>x.IsActive== true).Take(100).ToList();*/
            TempData["AdminSelectedCategoryId"] = categoryId;
            var selectedProducts = productList.Where(x=>x.Category.CategoryId==categoryId).ToList();
            if (categoryId == Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                return PartialView("_ProductListPartial", productList);
            }
            else
            {
                return PartialView("_ProductListPartial", selectedProducts);
            }

        }

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
                ViewBag.Success = "Student Added Successfully";
               
                return RedirectToAction("Index","Admin");
            }
            return View(addProductDto);
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var product = await _ecommerceDbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            ViewBag.CategoryList = await _ecommerceDbContext.Categories.ToListAsync();
            if (product != null)
            {
                var newProduct = new UpdateProductDto()
                {
                    ProdId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductPrice = product.ProductPrice,
                    ProductDes = product.ProductDes,
                    IsAvailable = product.IsAvailable,
                    IsTrending = product.IsTrending,
                   /* ProductImage = product.ProductImage,*/
                    CategoryRefId = product.CategoryRefId,
                    Category = product.Category,
                };
                return await Task.Run(() => View("Update", newProduct));
            }
            return RedirectToAction("GetAll");
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateProductDto updateProductDto)
        {
            var product = await _ecommerceDbContext.Products.FirstOrDefaultAsync(x => x.ProductId == updateProductDto.ProdId);
            ViewBag.CourseList = _ecommerceDbContext.Categories.ToList();

            string uniqueFileName = "";
            if (updateProductDto.ProductImage != null)
            {
                string uploadFoler = Path.Combine(_webHostEnvironment.WebRootPath, "image");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + updateProductDto.ProductImage.FileName;
                string filePath = Path.Combine(uploadFoler, uniqueFileName);
                updateProductDto.ProductImage.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            if (product != null)
            {
                product.ProductName = updateProductDto.ProductName;
                product.ProductPrice = updateProductDto.ProductPrice;
                product.ProductDes = updateProductDto.ProductDes;
                product.ProductImage = uniqueFileName;
                product.IsAvailable= updateProductDto.IsAvailable;
                product.IsTrending=updateProductDto.IsTrending;
                product.CategoryRefId= updateProductDto.CategoryRefId;
            }
                await _ecommerceDbContext.SaveChangesAsync();
            return RedirectToAction("Index","Admin");
        }

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
