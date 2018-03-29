using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Roham.Lib.Logger;
using Roham.Domain.Configs;
using Roham.Lib.Emails;

namespace Roham.Domain.Identity
{
    public class EmailService : IIdentityMessageService
    {
        private static ILogger Log = LoggerFactory.GetLogger<EmailService>();
        private readonly Func<IRohamConfigs> _configsResolver;

        public EmailService(Func<IRohamConfigs> configsResolver)
        {
            _configsResolver = configsResolver;
        }

        public Task SendAsync(IdentityMessage message)
        {   
            return new TaskFactory().StartNew(() =>
            {
                var configs = _configsResolver();
                var settings = new SmtpSettings(configs.SmtpHost, configs.SmtpPort, configs.SmtpEnableSsl, 
                                                configs.SmtpUsername, configs.SmtpPassword, configs.SmtpDomain);
                var email = new Email(settings, "no-replay@roham.com", new List<string> { message.Destination },
                                     message.Subject, message.Body);
                email.Send();
            });
        }
    }
}
