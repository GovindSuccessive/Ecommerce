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
