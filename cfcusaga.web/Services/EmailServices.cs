using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Cfcusaga.Web.Configuration;
using Microsoft.AspNet.Identity;
using SendGrid;

namespace Cfcusaga.Web.Services
{
    public class EmailService : IIdentityMessageService
    {
        private AppConfigurations _appConfig = new AppConfigurations();
        public async Task SendAsync(IdentityMessage message)
        {
            await configSendGridasync(message);
        }

        // Use NuGet to install SendGrid (Basic C# client lib) 
        private async Task configSendGridasync(IdentityMessage message)
        {
            var myMessage = new SendGridMessage();

            myMessage.AddTo(message.Destination);
            myMessage.From = new System.Net.Mail.MailAddress(_appConfig.FromEmail, _appConfig.FromName);
            myMessage.Subject = message.Subject;
            myMessage.Text = message.Body;
            myMessage.Html = message.Body;

            var transportWeb = new SendGrid.Web(ConfigurationManager.AppSettings["EmailApiKey"]);

            // Create a Web transport for sending email.

            // Send the email.
            await transportWeb.DeliverAsync(myMessage);
        }
    }
}
