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
            ViewBag.CategoryList = await _ecommerceDbContext.Categories.ToListAsync();
            var productList = await _ecommerceDbContext.Products.Include(x=>x.Category).Where(x=>x.IsActive==true).OrderByDescending(x=>x.IsTrending).Take(10).ToListAsync();
            return View(productList);
        }
       


    }
}
