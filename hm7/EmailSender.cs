using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading;

namespace hm7
{
    public class EmailSender : IEmailSender, IDisposable
    {
        private readonly MimeMessage mimeMessage;
        private bool IsDisposed;
        private EmailSenderConfig _config;

        //конструктор
        public EmailSender(IOptionsSnapshot<EmailSenderConfig> config)
        {
            _config = config.Value;
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

        public void SendEmail(string fromName, string fromEmail, string password, string toName, string toEmail, string subject, string body)
        {
            if (!IsDisposed)
            {
                //заполняем адресатов
                mimeMessage.From.Add(new MailboxAddress(fromName, fromEmail));
                mimeMessage.To.Add(new MailboxAddress(toName, toEmail));
                mimeMessage.Subject = subject;
                mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = body };
                //подключаемся к клиенту
                var smtpClient = new SmtpClient();
                smtpClient.Connect(_config.Host, _config.Port, _config.UseSsl);
                smtpClient.Authenticate(fromEmail, password);
                //отправляем сообщение
                //var response = smtpClient.Send(mimeMessage);
                //после получения ответа отключаемся от сервера
                smtpClient.Disconnect(true);
            }
        }

        public async Task SendEmailAsync(string fromName, string fromEmail, string password, string toName, string toEmail, string subject, string body, CancellationToken cancellationToken)
        {
            if (!IsDisposed)
            {
                //заполняем адресатов
                mimeMessage.From.Add(new MailboxAddress(fromName, fromEmail));
                mimeMessage.To.Add(new MailboxAddress(toName, toEmail));
                mimeMessage.Subject = subject;
                mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = body };
                //подключаемся к клиенту
                var smtpClient = new SmtpClient();
                await smtpClient.ConnectAsync(_config.Host, _config.Port, _config.UseSsl, cancellationToken);
                await smtpClient.AuthenticateAsync(fromEmail, password, cancellationToken);
                //отправляем сообщение
                var response = await smtpClient.SendAsync(mimeMessage, cancellationToken);
                //после получения ответа отключаемся от сервера
                await smtpClient.DisconnectAsync(true, cancellationToken);
            }
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                mimeMessage.Dispose();
                IsDisposed = true;
            }
            else return;
        }
    }
}
