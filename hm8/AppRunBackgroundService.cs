using Microsoft.Extensions.Hosting.Internal;

namespace hm8
{
    public class AppRunBackgroundService : BackgroundService
    {
        private readonly ILogger<AppRunBackgroundService> _logger;
        public AppRunBackgroundService(ILogger<AppRunBackgroundService> logger, ApplicationLifetime applicationLifetime) 
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            applicationLifetime.ApplicationStarted.Register(() =>
            {
                _logger.LogInformation("Сервер успешно запущен!");
            });
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Сервер успешно запущен!");
        }
    }
}
