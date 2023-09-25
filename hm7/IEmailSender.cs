namespace hm7
{
    public interface IEmailSender
    {
        public void SendEmail(string fromName, 
            string fromEmail,
            string password,
            string toName, 
            string toEmail,
            string subject, 
            string body);
        //с токеном отмены
        public Task SendEmailAsync(string fromName, 
            string fromEmail,
            string password,
            string toName, 
            string toEmail,
            string subject, 
            string body, 
            CancellationToken token);
    }
}
