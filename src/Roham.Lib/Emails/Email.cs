using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using Roham.Lib.Logger;
using System.Net.Sockets;

namespace Roham.Lib.Emails
{
    public class SmtpSettings
    {
        public SmtpSettings(string host, int port, bool enableSsl, string userName, string password, string domain = null)
        {
            Host = host;
            Port = port; // default is 25
            EnableSsl = enableSsl;
            UserName = userName;
            Password = password;
            Domain = domain;
        }

        public string Host { get; }
        public int Port { get; }
        public bool EnableSsl { get; }
        public string UserName { get; }
        public string Password { get; }
        public string Domain { get; }

        public override string ToString()
        {
            var ssl = EnableSsl ? "ssl://" : "";
            return $"[{ssl}{Host}:{Port}]";
        }
    }

    public class Email
    {
        private static ILogger Log = LoggerFactory.GetLogger<Email>();

        public Email(SmtpSettings settings, string from, List<string> tos, string subject, string body, bool isBodyHtml = true, List<string> ccs = null)
        {
            EmailSettings = settings;
            From = from;
            Tos = tos ?? new List<string>();
            Subject = subject;
            Body = body;
            IsBodyHtml = isBodyHtml;
            Ccs = ccs ?? new List<string>();
        }

        public SmtpSettings EmailSettings { get; }
        public string From { get; }
        public List<string> Tos { get; }
        public List<string> Ccs { get; }
        public string Subject { get; }
        public bool IsBodyHtml { get; }
        public string Body { get; }

        public void Send()
        {
            try
            {
                Objects.Requires(EmailSettings != null, () => new NullReferenceException("email settings is null"));
                Objects.Requires(!string.IsNullOrWhiteSpace(From), () => new NullReferenceException("email From is null or empty"));
                Objects.Requires(Tos.Count > 0, () => new NullReferenceException("email To is empty"));
                Objects.Requires(!string.IsNullOrWhiteSpace(Subject), () => new NullReferenceException("email Subject is null or empty"));

                var message = new MailMessage();
                message.From = new MailAddress(From);
                Tos.ForEach(to => message.To.Add(new MailAddress(to)));
                message.Subject = Subject;
                message.Body = Body;
                message.IsBodyHtml = IsBodyHtml;
                Ccs.ForEach(cc => message.CC.Add(cc));

                using (var smtpClient = new SmtpClient(EmailSettings.Host, EmailSettings.Port) { EnableSsl = EmailSettings.EnableSsl })
                {
                    if (!string.IsNullOrWhiteSpace(EmailSettings.UserName))
                    {
                        smtpClient.Credentials = string.IsNullOrWhiteSpace(EmailSettings.Domain)
                            ? new NetworkCredential(EmailSettings.UserName, EmailSettings.Password)
                            : new NetworkCredential(EmailSettings.UserName, EmailSettings.Password, EmailSettings.Domain);
                    }

                    smtpClient.Send(message);
                }
            }
            catch(Exception ex)
            {
                Log.Error($"Sending email {this} with settings {EmailSettings} failed", ex);
                throw;
            }
        }

        public bool TrySend(out string errorMessage)
        {
            errorMessage = null;
            try
            {
                Send();
                return true;
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public static bool TryPingHost(string host, int port, out string errorMessage)
        {
            errorMessage = "";
            using (var tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect(host, port);
                    return true;
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    return false;
                }
            }
        }

        public override string ToString()
        {
            var tosList = string.Join(",", Tos);
            var ccsList = string.Join(",", Ccs);
            return $"[subject:{Subject} from:{From} to:{tosList} cc:{ccsList} isBodyHtml:{IsBodyHtml}]";
        }
    }
}
