using EcommerceManagement.Models.Domain;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;

namespace EcommerceManagement.EmailServices
{
    public class EmailSender : IEmailSender
    {
        private readonly SignInManager<UserModel> _signInManager;
        public readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<UserModel> _userManager;

        public EmailSender(SignInManager<UserModel> signInManager, UserManager<UserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "user21819@outlook.com";
            var pw = "sz37lhcv";

            var customer = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
            Credentials = new NetworkCredential(mail, pw)
            };

            return customer.SendMailAsync(
                new MailMessage(from: mail,
                                to: email,
                                subject,
                                message
                                ));
            
            /*   var gmailUsername = ConfigurationManager.AppSettings["GmailUsername"];
               var gmailPassword = ConfigurationManager.AppSettings["GmailPassword"];

               var client = new SmtpClient("smtp.gmail.com", 587)
               {
                   EnableSsl = true,
                   Credentials = new NetworkCredential(gmailUsername, gmailPassword)
               };

               return client.SendMailAsync(
                   new MailMessage(from: gmailUsername, to: email, subject, message));*/


        }
    }
}
