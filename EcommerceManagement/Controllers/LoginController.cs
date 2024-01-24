using EcommerceManagement.Data;
using EcommerceManagement.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace EcommerceManagement.Controllers
{
    public class LoginController : Controller
    {
        private readonly EcommerceDbContext _ecommerceDbContext;

        public LoginController(EcommerceDbContext ecommerceDbContext)
        {
            _ecommerceDbContext = ecommerceDbContext;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Loged(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the username (email or phone number) exists in the database
                var user = await _ecommerceDbContext.Users.FirstOrDefaultAsync(u => u.Email == model.UserName || u.PhoneNo == model.UserName);

                if (user != null)
                {
                    // Verify the password
                    if (VerifyPassword(model.Password, user.Password))
                    {
                        // Password is correct, proceed with login
                        // You can set authentication cookies or tokens here
                        return RedirectToAction("Index", "Admin"); // Redirect to the home page after successful login
                    }
                }

                // Invalid username or password
                ModelState.AddModelError(string.Empty, "Invalid username or password");
            }

            return View("Index", model); // Return the login view with errors
        }

        private bool VerifyPassword(string password1, string password2)
        {
            // Implement your password verification logic here
            // For example, you can use hashing and salting techniques to compare passwords
            // Here's a simple example for demonstration purposes only
            
            return password1 == password2;
        }
    }
}
