using EcommerceManagement.Data;
using EcommerceManagement.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceManagement.Controllers
{
    public class FavoritController : Controller
    {
        private readonly EcommerceDbContext _ecommerceDbContext;
        private readonly SignInManager<UserModel> _signInManager;

        public FavoritController(EcommerceDbContext ecommerceDbContext,SignInManager<UserModel> signInManager)
        {
            _ecommerceDbContext = ecommerceDbContext;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            var favoritItem = _ecommerceDbContext.Favorits.Include(x=>x.Product).ToList();
            return View(favoritItem);
        }

        [HttpGet]
        public async Task<IActionResult> AddToFavorit([FromRoute] Guid id)
        {
            if (_signInManager.IsSignedIn(User))
            {

                if (id != Guid.Empty)
                {
                    var user = _signInManager.UserManager.GetUserId(User);
                    var isPersent = _ecommerceDbContext.Favorits.Where(x=>x.UserRefId ==user).FirstOrDefault(x=>x.ProductRefId == id);

                    if (isPersent==null)
                    {
                        var newFavorit = new FavoritModel()
                        {
                            FavoritId = Guid.NewGuid(),
                            ProductRefId = id,
                            UserRefId = user,
                        };
                        await _ecommerceDbContext.Favorits.AddAsync(newFavorit);
                        await _ecommerceDbContext.SaveChangesAsync();
                    }
                }
                return RedirectToAction("Index","Home");
            }
            return RedirectToAction("Login", "Authentication");
        }

        [HttpGet]
        public async Task<IActionResult> RemoveFromFavorit([FromRoute] Guid id)
        {
            if (id != Guid.Empty)
            {
                var favoritItems = _ecommerceDbContext.Favorits.FirstOrDefault(x => x.FavoritId == id);
                if (favoritItems != null)
                {
                    _ecommerceDbContext.Favorits.Remove(favoritItems);
                    await _ecommerceDbContext.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

        

    }
}
