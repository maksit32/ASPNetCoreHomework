using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading;

namespace hm8
{
    public class EmailSender : IEmailSender, IDisposable
    {
        private readonly MimeMessage mimeMessage;
        private bool IsDisposed;
        private EmailSenderConfig _config;
        private readonly ILogger<EmailSender> _logger;
        private short countSend; //количество retry

        //конструктор
        public EmailSender(IOptionsSnapshot<EmailSenderConfig> config, ILogger<EmailSender> logger)
        {
            _config = config.Value;
            mimeMessage = new MimeMessage();
            IsDisposed = false;
            countSend = 2;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger.LogInformation("EmailSender created");
        }

        public async Task DisposeAsync()
        {
            if (!IsDisposed)
            {
                await Task.Run(() => { mimeMessage.Dispose(); });
                IsDisposed = true;
                _logger.LogInformation("EmailSender disposed");
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
                _logger.LogInformation("smtpClient connected");
                smtpClient.Authenticate(fromEmail, password);
                _logger.LogInformation("smtpClient authenticated");
                //отправляем сообщение
                //var response = smtpClient.Send(mimeMessage);
                //после получения ответа отключаемся от сервера
                smtpClient.Disconnect(true);
                _logger.LogInformation("smtpClient disconnected");
            }
        }

        public async Task SendEmailAsync(string fromName, string fromEmail, string password, string toName, string toEmail, string subject, string body, CancellationToken cancellationToken)
        {
            try
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
                    _logger.LogInformation("smtpClient connected");
                    await smtpClient.AuthenticateAsync(fromEmail, password, cancellationToken);
                    _logger.LogInformation("smtpClient authenticated");
                    //отправляем сообщение
                    var response = await smtpClient.SendAsync(mimeMessage, cancellationToken);
                    _logger.LogInformation("smtpClient sent a message  " + mimeMessage.TextBody);
                    countSend = 2; //count send
                    //после получения ответа отключаемся от сервера
                    await smtpClient.DisconnectAsync(true, cancellationToken);
                    _logger.LogInformation("smtpClient disconnected");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error", ex);
                if (countSend > 0)
                {
                    countSend--;
                    SendEmailAsync(fromName, fromEmail, password, toName, toEmail, subject, body, cancellationToken);
                }
            }
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                mimeMessage.Dispose();
                IsDisposed = true;
                _logger.LogInformation("EmailSender disposed");
            }
            else return;
        }
    }
}
