using System;
using Error;
using MailKit.Net.Smtp;
using MimeKit;

namespace Variance
{
    public static class Email
    {
        public static void Send(string host, string port, bool ssl, string subject, string messageContent, string address, string password)
        {
            if ((host != "") && (port != "") && (address != "") && (password != ""))
            {
                try
                {
                    SmtpClient client = new SmtpClient {ServerCertificateValidationCallback = (s, c, h, e) => true};
                    client.Connect(host, Convert.ToInt32(port), ssl);
                    MimeMessage message = new MimeMessage
                    {
                        Subject = subject, Body = new TextPart("plain") {Text = messageContent}
                    };
                    message.From.Add(new MailboxAddress(address, address));
                    message.To.Add(new MailboxAddress(address, address));
                    client.Authenticate(address, password);
                    client.Send(message);
                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    ErrorReporter.showMessage_OK(ex.Message, "Email problem");
                }
            }
        }
    }
}
