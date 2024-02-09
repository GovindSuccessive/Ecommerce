using EcommerceManagement.Constant;
using EcommerceManagement.Data;
using EcommerceManagement.Models.Domain;
using EcommerceManagement.Models.Dto;
using EcommerceManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SendGrid.Helpers.Mail;

namespace EcommerceManagement.Controllers
{
    public class Authentication : Controller 
    {
        private readonly SignInManager<UserModel> _signInManager;
        private readonly UserManager<UserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly EmailSender _emailSender;

        public Authentication(SignInManager<UserModel> signInManager, 
            UserManager<UserModel> userManager, 
            RoleManager<IdentityRole> roleManager,
            EmailSender emailSender
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAllUser()
        {
            var users = _userManager.Users.Where(x=>x.Role=="User");
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
            var changingUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == changePasswordByAdmin.UserName);


            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(changingUser, changePasswordByAdmin.OldPassword, changePasswordByAdmin.NewPassword);
                if (result.Succeeded)
                {
                    user.Password = changePasswordByAdmin.NewPassword;
                    user.ConfirmPassword = changePasswordByAdmin.NewPassword;
                    changingUser.Password = changePasswordByAdmin.NewPassword;
                    changingUser.ConfirmPassword = changePasswordByAdmin.ConfirmNewPassword;
                    string htmlMessage = $@"
                                      <html>
                                      <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; color: #333; padding: 20px;'>
                                          <div style='max-width: 600px; margin: 0 auto; background-color: #fff; padding: 20px; border-radius: 5px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
                                              <div class=""navbar-brand"" style=""background-color: black; padding: 10px;"">
                                                 <span style=""font-style: italic; font-weight: bold; font-size: 1.5rem; color: #1d58af;"">Easy</span>
                                                 <span style=""font-weight: bold; font-size: 1.5rem; color: white;"">Mart</span>
                                             </div>
                                              <p>Hello {changingUser.FirstName} {changingUser.LastName},</p>
                                              <p>Your password has been changed by the admin. Below are the details:</p>
                                              <ul>
                                                  <li>New Password: {changePasswordByAdmin.NewPassword}</li>
                                              </ul>
                                              <p>Thank you!</p>
                                              <!-- Add your images here -->
                                      
                                              <!-- Company Information -->
                                              <div style='margin-top: 20px; border-top: 1px solid #ddd; padding-top: 10px; font-size: 12px; color: #777;'>
                                                  <p>EasyMart Ecommerce Company</p>
                                                  <p>123 Main Street, Cityville</p>
                                                  <p>Email: info@easymart.com</p>
                                                  <p>Phone: (123) 456-7890</p>
                                              </div>
                                          </div>
                                      </body>
                                      </html>";

                    await _userManager.UpdateAsync(changingUser);
                    await _emailSender.SendEmail("Passsword Change", changingUser.Email, changingUser.FirstName + changingUser.LastName, htmlMessage);
                    ModelState.Clear();
                    return RedirectToAction("GetAllUser");
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

        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string email)
        {
            var isVerified=_userManager.FindByEmailAsync(email);
            var OTP=Guid.NewGuid().ToString();
            TempData["OTP"] = OTP;

            if (ModelState.IsValid)
            {
                if (isVerified != null)
                {
                    var message = $@"
                                      <html>
                                      <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; color: #333; padding: 20px;'>
                                          <div style='max-width: 600px; margin: 0 auto; background-color: #fff; padding: 20px; border-radius: 5px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);'>
                                              <div class=""navbar-brand"" style=""background-color: black; padding: 10px;"">
                                                 <span style=""font-style: italic; font-weight: bold; font-size: 1.5rem; color: #1d58af;"">Easy</span>
                                                 <span style=""font-weight: bold; font-size: 1.5rem; color: white;"">Mart</span>
                                             </div>
                                              <p>Hello {isVerified.Result.FirstName} {isVerified.Result.LastName},</p>
                                              <p>Your One Time Password is Genrated Below</p>
                                               <p>Paste it to the Required Fome Fields</p>
                                              <ul>
                                                  <li>OTP:{OTP} </li>
                                              </ul>
                                              <p>Thank you!</p>
                                              <!-- Add your images here -->
                                      
                                              <!-- Company Information -->
                                              <div style='margin-top: 20px; border-top: 1px solid #ddd; padding-top: 10px; font-size: 12px; color: #777;'>
                                                  <p>EasyMart Ecommerce Company</p>
                                                  <p>123 Main Street, Cityville</p>
                                                  <p>Email: info@easymart.com</p>
                                                  <p>Phone: (123) 456-7890</p>
                                              </div>
                                          </div>
                                      </body>
                                      </html>";

                  await  _emailSender.SendEmail("Password Change Reques", isVerified.Result.UserName, isVerified.Result.FirstName + isVerified.Result.LastName, message);
                    return RedirectToAction("VerifyOtp");
                }
                ModelState.AddModelError("", "User Not Found");
                return View(email);
            }
            ModelState.AddModelError("", "Invalid Input Fields");
            return View(email);
        }

        [HttpGet]
        public  IActionResult VerifyOtp(string OTP)
        {
            if (ModelState.IsValid)
            {
                if (OTP != null)
                {
                    if (TempData.TryGetValue("OTP", out var sentOTP))
                    {
                        ViewBag.OldOtp= sentOTP;
                    }
                    if (ViewBag.OldOtp == OTP)
                    {
                       return RedirectToAction("ChangeForgatePassword");
                    }
                }
            }
            ModelState.AddModelError("", "Invalid OTP");
            return View(OTP);
        }

        [HttpGet]
        public IActionResult ChangeForgatePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangeForgatePassword(ChangeForgatePassword changeForgatePassword)
        {
            
        }
       
    }


}
