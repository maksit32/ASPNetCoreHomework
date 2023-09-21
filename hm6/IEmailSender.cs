namespace hm6
{
    public interface IEmailSender
    {
        public void SendEmail(string fromName, 
            string fromEmail,
            string password,
            string toName, 
            string toEmail,
            string subject, 
            string body, 
            bool useSsl);
        //с токеном отмены
        public Task SendEmailAsync(string fromName, 
            string fromEmail,
            string password,
            string toName, 
            string toEmail,
            string subject, 
            string body, 
            bool useSsl, 
            CancellationToken token);
    }
}
