using EcommerceManagement.Data;
using EcommerceManagement.Models;
using EcommerceManagement.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EcommerceManagement.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class HomeController : Controller
    {
       
       // private readonly ILogger<HomeController> _logger;
        private readonly EcommerceDbContext _ecommerceDbContext;

        public HomeController(EcommerceDbContext ecommerceDbContext)
        {
            //_logger = logger;
            _ecommerceDbContext = ecommerceDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var productList = await _ecommerceDbContext.Products.ToListAsync();
            var categoryList = await _ecommerceDbContext.Categories.ToListAsync();
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
                           ProductCreated = product.ProductCreated,
                           IsTrending = product.IsTrending,
                           CategoryId = category.CategoryId,
                           CategoryName = category.CategoryName
                       }).OrderByDescending(x => x.ProductCreated).ToList();
            return View(categoryProduct);
        }


    }
}
