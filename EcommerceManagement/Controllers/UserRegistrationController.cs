using Microsoft.AspNetCore.Mvc;

namespace EcommerceManagement.Controllers
{
    public class UserRegistrationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
