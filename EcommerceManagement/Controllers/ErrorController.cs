using Microsoft.AspNetCore.Mvc;

namespace EcommerceManagement.Controllers
{
    public class ErrorController : Controller
    {

        [Route("/Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            if (statusCode == 404)
            {
                // Handle 404 errors here (e.g., show a custom error page)
                return View("NotFound");
            }

            // Handle other errors
            return View("Error");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
