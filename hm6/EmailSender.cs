using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Threading;

namespace hm6
{
    public class EmailSender : IEmailSender, IDisposable
    {
        private readonly MimeMessage mimeMessage;
        private readonly string server;
        private readonly int port;
        private bool IsDisposed;

        //конструктор
        public EmailSender()
        {
            server = "smtp.beget.com";
            port = 25;
            mimeMessage = new MimeMessage();
            IsDisposed = false;
        }

        public async Task DisposeAsync()
        {
            if (!IsDisposed)
            {
                await Task.Run(() => { mimeMessage.Dispose(); });
                IsDisposed = true;
            }
            else return;
        }

        public bool GetIsDisposed()
        {
            return IsDisposed;
        }

        public void SendEmail(string fromName, string fromEmail, string password, string toName, string toEmail, string subject, string body, bool useSsl)
        {
            if(!IsDisposed)
            {
                //заполняем адресатов
                mimeMessage.From.Add(new MailboxAddress(fromName, fromEmail));
                mimeMessage.To.Add(new MailboxAddress(toName, toEmail));
                mimeMessage.Subject = subject;
                mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = body };
                //подключаемся к клиенту
                var smtpClient = new SmtpClient();
                smtpClient.Connect(server, port, useSsl);
                smtpClient.Authenticate(fromEmail, password);
                //отправляем сообщение
                var response = smtpClient.Send(mimeMessage);
                //_logger.LogInformation($"Ответ  сервера: {response}");
                //после получения ответа отключаемся от сервера
                smtpClient.Disconnect(true);
            }
        }

        public async Task SendEmailAsync(string fromName, string fromEmail, string password, string toName, string toEmail, string subject, string body, bool useSsl, CancellationToken cancellationToken)
        {
            if(!IsDisposed)
            {
                //заполняем адресатов
                mimeMessage.From.Add(new MailboxAddress(fromName, fromEmail));
                mimeMessage.To.Add(new MailboxAddress(toName, toEmail));
                mimeMessage.Subject = subject;
                mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = body };
                //подключаемся к клиенту
                var smtpClient = new SmtpClient();
                await smtpClient.ConnectAsync(server, port, useSsl, cancellationToken);
                await smtpClient.AuthenticateAsync(fromEmail, password, cancellationToken);
                //отправляем сообщение
                var response = await smtpClient.SendAsync(mimeMessage, cancellationToken);
                //_logger.LogInformation($"Ответ  сервера: {response}");
                //после получения ответа отключаемся от сервера
                await smtpClient.DisconnectAsync(true, cancellationToken);
            }
        }

        public void Dispose()
        {
            mimeMessage.Dispose();
            IsDisposed = true;
        }
    }
}
