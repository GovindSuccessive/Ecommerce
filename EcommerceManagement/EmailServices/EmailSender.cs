using EcommerceManagement.Models.Domain;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;

namespace EcommerceManagement.EmailServices
{
    public class EmailSender:IEmailSender
    {
        private readonly SignInManager<UserModel> _signInManager;
        public readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<UserModel> _userManager;

        public EmailSender(SignInManager<UserModel> signInManager,UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "tutorialseu-dev@outlook.com";
            var pw = "Test12345678!";

            var customer = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = false,
                Credentials = new NetworkCredential(mail, pw)
            };

            return customer.SendMailAsync(
                new MailMessage(from: mail,
                                to: email,
                                subject,
                                message
                                ));


        }
    }
}
