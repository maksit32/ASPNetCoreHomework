using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Threading;

namespace hm5
{
    public class EmailSender : IEmailSender
    {
        private readonly MimeMessage mimeMessage;
        private readonly string server;
        private readonly int port;

        private readonly ILogger _logger;
        //конструктор
        public EmailSender(ILogger logger)
        {
            server = "smtp.beget.com";
            port = 25;
            mimeMessage = new MimeMessage();
            _logger = logger;
        }
        public void SendEmail(string fromName, string fromEmail, string password, string toName, string toEmail, string subject, string body, bool useSsl)
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
            _logger.LogInformation($"Ответ  сервера: {response}");
            //после получения ответа отключаемся от сервера
            smtpClient.Disconnect(true);
        }

        public async Task SendEmailAsync(string fromName, string fromEmail, string password, string toName, string toEmail, string subject, string body, bool useSsl, CancellationToken cancellationToken)
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
            _logger.LogInformation($"Ответ  сервера: {response}");
            //после получения ответа отключаемся от сервера
            await smtpClient.DisconnectAsync(true, cancellationToken);
        }
    }
}
