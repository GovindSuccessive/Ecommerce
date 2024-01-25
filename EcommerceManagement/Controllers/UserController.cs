using Microsoft.AspNetCore.Mvc;

namespace EcommerceManagement.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
