using EcommerceManagement.Data;
using EcommerceManagement.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceManagement.Controllers
{
    [Authorize(Roles = "User")]
    public class CartController : Controller
    {
        private readonly EcommerceDbContext _ecommerceDbContext;
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;

        public CartController(EcommerceDbContext ecommerceDbContext, UserManager<UserModel> userManager, SignInManager<UserModel> signInManager)
        {
            _ecommerceDbContext = ecommerceDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            var userId = _signInManager.UserManager.GetUserId(User);
            var cartItem = _ecommerceDbContext.Carts.Include(X => X.Product).Where(x=>x.UserRefId==userId).ToList();

            return View(cartItem);
        }
        
        [HttpGet]
        public async Task<IActionResult> AddToCart([FromRoute]Guid id)
        {
            if (_signInManager.IsSignedIn(User))
            {

                if (id != Guid.Empty)
                {
                    var user = _signInManager.UserManager.GetUserId(User);
                    var userId = _signInManager.UserManager.FindByIdAsync(user).Result.UserName;
                    var existingCart = _ecommerceDbContext.Carts.Where(x=>x.UserRefId==user).FirstOrDefault(x => x.ProductId == id);

                    if (existingCart != null)
                    {
                        existingCart.CartItems++;
                    }
                    else
                    {
                        var newcart = new CartModel()
                        {
                            CartId = Guid.NewGuid(),
                            CartItems = 1,
                            ProductId = id,
                            ProductRefId = id,
                            UserRefId = user,
                        };
                        await _ecommerceDbContext.Carts.AddAsync(newcart);
                    }
                    await _ecommerceDbContext.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login", "Authentication");
        }

        [HttpGet]
        public async Task<IActionResult> OrderDelet([FromRoute]Guid id)
        {
            if(id != Guid.Empty)
            {
                var CartItems = _ecommerceDbContext.Carts.FirstOrDefault(x=>x.CartId==id);
                if (CartItems != null)
                {
                    _ecommerceDbContext.Carts.Remove(CartItems);
                    await _ecommerceDbContext.SaveChangesAsync();
                }
            }
            TempData["productCartRemove"] = "Product Remove Successfully";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DecrementCartItem(Guid id)
        {
            var LoginUser = _signInManager.UserManager.GetUserAsync(User).Result.Id;
            var existingProduct = _ecommerceDbContext.Carts.Where(x => x.UserRefId == LoginUser).FirstOrDefault(x => x.ProductId == id);
            
            if (existingProduct != null)
            {
                if (existingProduct.CartItems == 1)
                {
                   return  RedirectToAction("OrderDelet", new { id = existingProduct.CartId });   
                }
                existingProduct.CartItems--;
                await _ecommerceDbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> IncrementCartItem(Guid id)
        {
            var LoginUser = _signInManager.UserManager.GetUserAsync(User).Result.Id;
            var existingProduct = _ecommerceDbContext.Carts.Where(x => x.UserRefId == LoginUser).FirstOrDefault(x => x.ProductId == id);

            if (existingProduct != null)
            {
                existingProduct.CartItems++;
                await _ecommerceDbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
