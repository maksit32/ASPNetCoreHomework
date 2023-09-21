namespace hm6
{
    //фоном будет оправлять на почту сообщения
    public class BackgroundEmailMemodySender : BackgroundService
    {
        //для отправки на email
        private readonly IEmailSender emailSender;
        //для проверки памяти процесса
        private readonly IMemoryUsedChecker memoryCheck;
        private readonly TimeSpan timeout;
        //конструктор (timeout --- время 60 минут поставим) + DIP
        public BackgroundEmailMemodySender(IEmailSender emailSender, IMemoryUsedChecker memoryChecker, TimeSpan timeout)
        {
            this.emailSender = emailSender;
            this.timeout = timeout;
            this.memoryCheck = memoryChecker;
        }

        //переопределение метода BackgroundService (будет выполнять это действие)
        protected override async Task ExecuteAsync(CancellationToken canclellationToken)
        {
            //если токен отмены не вызван
            while (!canclellationToken.IsCancellationRequested)
            {
                try
                {
                    //отправляем асинхронно сообщение
                    //check memory вернул string памяти
                    await emailSender.SendEmailAsync("Kornilev M.", "asp2022pd011@rodion-m.ru","6WU4x2be", 
                        "Maxim", "kornilev.maxim@mail.ru", "Memory of App",
                        memoryCheck.CheckMemoryApp(), false, canclellationToken);
                    await Task.Delay(timeout, canclellationToken); //выполнится через какое-то время
                }
                catch (Exception)
                {
                    throw new Exception("Ошибка!");
                }
            }
        }
    }
}
