using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using PTApplication.Models.Global;

namespace PTApplication.Controllers
{
    [Route("PTAPI/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class HomeController : ControllerBase
    {
        [HttpPost]
        public IActionResult Send()
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("laurel.schmidt@ethereal.email"));
            email.To.Add(MailboxAddress.Parse("laurel.schmidt@ethereal.email"));
            email.Subject = "OPT Testing";
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = "This is a test email body" };

            using var smtp = new SmtpClient();
            smtp.Connect("smtp.ethereal.email",587,MailKit.Security.SecureSocketOptions.StartTls);

            smtp.Authenticate("laurel.schmidt@ethereal.email", "3KDeuWBeQeHJadXvPZ");
            smtp.SendAsync(email);
            smtp.Disconnect(true);

            return Ok();
        }
    }
}
