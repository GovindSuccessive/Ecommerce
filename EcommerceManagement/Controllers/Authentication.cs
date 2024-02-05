using EcommerceManagement.Constant;
using EcommerceManagement.Data;
using EcommerceManagement.EmailServices;
using EcommerceManagement.Models.Domain;
using EcommerceManagement.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceManagement.Controllers
{
    public class Authentication : Controller 
    {
        private readonly SignInManager<UserModel> _signInManager;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;

        public Authentication(SignInManager<UserModel> signInManager, 
            UserManager<UserModel> userManager, 
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
           _emailSender = emailSender;
        }


        public IActionResult Login()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                bool EmailExist = _userManager.Users.AnyAsync(x => x.UserName == model.Username).Result;
                if (EmailExist) {
                
                    bool checkActivation = _userManager.Users.First(x => x.Email == model.Username).IsActivate;
                    //login
                    if (checkActivation)
                    {
                        var result = await _signInManager.PasswordSignInAsync(model.Username!, model.Password!, model.RememberMe, false);
                   
                        if (result.Succeeded && checkActivation)
                        {
                            if (User.IsInRole("Admin"))
                            {
                                return RedirectToAction("Index", "Admin");
                            }
                            if (User.IsInRole("User"))
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid login attempt");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Your Account is Deactivated! Please Contact to Admin");
                        
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Your Account Does not Exits");
                }

               
                return View(model);
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(AddUserDto model)
        {
            if (ModelState.IsValid)
            {
                var user = new UserModel
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNo = model.PhoneNo,
                    Address = model.Address,
                    IsActivate=true,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword,

                };

                var result = await _userManager.CreateAsync(user, model.Password!);

                if (result.Succeeded)
                {
                    if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        await _userManager.AddToRoleAsync(user, Roles.User.ToString());
                        return RedirectToAction("GetAllUser", "Authentication");
                    }
                    await _userManager.AddToRoleAsync(user, Roles.User.ToString());
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult GetAllUser()
        {
            var users = _userManager.Users;
            return View(users);
        }

        [HttpGet]
        public IActionResult GetProfile()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var userId = _signInManager.UserManager.GetUserId(User);
                var user = _signInManager.UserManager.FindByIdAsync(userId).Result;
                return View(user);
            }
           return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user != null)
            {
               var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
                if (result.Succeeded)
                {
                    user.Password=changePasswordDto.NewPassword;
                    user.ConfirmPassword = changePasswordDto.NewPassword;

                    await _userManager.UpdateAsync(user);
                    ModelState.Clear();
                    return RedirectToAction("GetProfile");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }


            }
            return View(changePasswordDto);
        }

        [HttpGet]
        public IActionResult ChangePasswordByAdmin(Guid id)
        {
            var user = _userManager.Users.First(x => x.Id == id.ToString());
            var newUser = new ChangePasswordByAdmin()
            {
                OldPassword = user.Password,
                UserName = user.UserName
            };
            return View(newUser);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePasswordByAdmin(ChangePasswordByAdmin changePasswordByAdmin)
        {
            var user = await _userManager.GetUserAsync(User);
            var changingUser = await _userManager.Users.FirstAsync(x=>x.UserName==changePasswordByAdmin.UserName);


            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(changingUser, changePasswordByAdmin.OldPassword, changePasswordByAdmin.NewPassword);
                if (result.Succeeded)
                {
                    user.Password = changePasswordByAdmin.NewPassword;
                    user.ConfirmPassword = changePasswordByAdmin.NewPassword;
                    var message = "NewPassword : = " + changePasswordByAdmin.NewPassword;
                    await _userManager.UpdateAsync(changingUser);
                    await _emailSender.SendEmailAsync(changePasswordByAdmin.UserName, "PassWord Changed BY Admin", message);
                    ModelState.Clear();
                    return RedirectToAction("GetProfile");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }


            }
            return View(changePasswordByAdmin);
        }


        [HttpGet]
        public async Task<IActionResult> UpdateUsers(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErroMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new UpdateUserModel()
            {
                Id=Guid.Parse(user.Id),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                PhoneNo = user.PhoneNo,
                Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUsers(UpdateUserModel updateUserModel)
        {
            var user = await _userManager.FindByIdAsync(updateUserModel.Id.ToString());
            if (user == null)
            {
                ViewBag.ErroMessage = $"User with Id = {updateUserModel.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                user.FirstName = updateUserModel.FirstName;
                user.LastName = updateUserModel.LastName;
                user.Address = updateUserModel.Address;
                user.PhoneNo = updateUserModel.PhoneNo;
                

                var result = await _userManager.UpdateAsync(user);

                if(result.Succeeded)
                {
                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("GetAllUser");
                    }
                    else
                    {
                        return RedirectToAction("GetProfile");
                    }
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
                return View(updateUserModel);
            }

        }


        [HttpGet]
        public async Task<IActionResult> Activate(Guid id)
        {
            var user =await  _userManager.FindByIdAsync(id.ToString());

            if (user != null)
            {
                user.IsActivate = true;
               await  _userManager.UpdateAsync(user);
            }
            return RedirectToAction("GetAllUser");
        }

        [HttpGet]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user != null)
            {
                user.IsActivate = false;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("GetAllUser");
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]

        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            
            if(user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await _userManager.DeleteAsync(user);

                if(result.Succeeded)
                {
                    return RedirectToAction("Logout");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return RedirectToAction("GetProfile");
            }
        }



    }
}
