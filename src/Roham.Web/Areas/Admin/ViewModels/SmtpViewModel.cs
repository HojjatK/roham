using Roham.Lib.Emails;

namespace Roham.Web.Areas.Admin.ViewModels
{
    public class SmtpViewModel
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }

        public string TestEmailFromAddress { get; set; }
        public string TestEmailToAddress { get; set; }

        public SmtpSettings ConvertToSettings()
        {
            return new SmtpSettings(Host, Port, EnableSsl, UserName, Password, Domain);
        }
    }
}
