using EcommerceManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceManagement.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly EcommerceDbContext _ecommerceDbContext;

        public AdminController(EcommerceDbContext ecommerceDbContext)
        {
            _ecommerceDbContext = ecommerceDbContext;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.CategoryList=await _ecommerceDbContext.Categories.ToListAsync();
            return View();
        }








    }
}
