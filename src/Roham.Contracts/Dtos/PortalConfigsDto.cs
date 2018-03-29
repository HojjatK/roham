using Roham.Lib.Domain;
using Roham.Lib.Emails;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Roham.Contracts.Dtos
{
    public class PortalSmtpSettingsDto
    {
        public bool SmtpEnabled { get; set; }

        [Description("The server name for your SMTP server.")]
        [MaxLength(Lengths.LongName)]
        public string SmtpHost { get; set; }

        [DefaultValue("21")]
        [Description("The port that your SMTP server listens on.")]        
        public int SmtpPort { get; set; }

        [DefaultValue("")]
        [Description("If your SMTP server requires authentication, enter your username here, or leave it empty.")]
        [MaxLength(Lengths.Name)]
        public string SmtpUsername { get; set; }

        [DefaultValue("")]
        [Description("If your SMTP server requires authentication, enter your password here, or leave it empty.")]
        [MaxLength(Lengths.Password)]        
        public string SmtpPassword { get; set; }

        [DefaultValue("")]
        [Description("If your SMTP server requires authentication, enter your user domain here, or leave it empty.")]
        [MaxLength(Lengths.LongName)]
        public string SmtpDomain { get; set; }

        [DefaultValue(false)]
        [Description("Whether SSL should be used when sending emails")]
        public bool SmtpUseSsl { get; set; }

        [DefaultValue("")]
        [MaxLength(Lengths.Email)]
        public string SmtpFrom { get; set; }

        public SmtpSettings ToSmtpSettings()
        {
            return new SmtpSettings(SmtpHost, SmtpPort, SmtpUseSsl, SmtpUsername, SmtpPassword, SmtpDomain);
        }
    }

    public class PortalCacheSettingsDto
    {
        public bool ExtCacheEnabled { get; set; }

        [MaxLength(Lengths.LongName)]
        public string CacheProvider { get; set; }
        
        [MaxLength(Lengths.LongName)]
        public string CacheHost { get; set; }
        
        public int CachePort { get; set; }

        [MaxLength(Lengths.Password)]
        public string CachePassword { get; set; }
        
        [DefaultValue(false)]
        public bool CacheUseSsl { get; set; }

        public List<KeyValuePair<string, string>> AvailableCacheProviders { get; set; }
    }
}