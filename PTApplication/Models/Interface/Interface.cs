using PTApplication.Models.Global;
using System.Threading.Tasks;

namespace PTApplication.Models.Interface
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
