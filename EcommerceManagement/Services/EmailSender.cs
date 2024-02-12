using SendGrid.Helpers.Mail;
using SendGrid;

namespace EcommerceManagement.Services
{
    public class EmailSender
    {
        public async Task SendEmail(string sub,string toMail, string username,string message)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);
            var from_email = new EmailAddress("iamgovinda100@gmail.com","Admin");
            var subject = sub;
            var to_email = new EmailAddress(toMail, username);
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = message;
            var msg = MailHelper.CreateSingleEmail(from_email, to_email, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
        }
    }
}
