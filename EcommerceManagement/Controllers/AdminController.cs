using Microsoft.AspNetCore.Mvc;

namespace EcommerceManagement.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
