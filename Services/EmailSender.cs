using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace AzureAppINTEX.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // This is a no-op email sender; it does nothing.
            return Task.CompletedTask;
        }
    }
}
