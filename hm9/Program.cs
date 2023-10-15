using hm8;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text;



Log.Logger = new LoggerConfiguration().WriteTo.Console()
                .CreateLogger();
Log.Information("Server started. Info by SerilogInfo");


try
{
    var builder = WebApplication.CreateBuilder(args);
    //добавление лог-файла
    builder.Host.UseSerilog((ctx, conf) =>
    {
        conf.MinimumLevel.Information()
        .WriteTo.Console()
        .MinimumLevel.Information()
        .WriteTo.File(
            "Logs/log.txt",
            rollingInterval: RollingInterval.Day,
            encoding: Encoding.UTF8)
        .MinimumLevel.Information();
    });


    //добавляем swager, настраиваем сервисы
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddOptions<EmailSenderConfig>()
                    .BindConfiguration("EmailSenderConfig")
                    .ValidateDataAnnotations()
                    .ValidateOnStart();


    //подключаем сервис
    builder.Services.AddSingleton<ApplicationLifetime>(); //без него ошибка!
    builder.Services.AddHostedService<AppRunBackgroundService>(p =>
    {
        return new AppRunBackgroundService(p.GetRequiredService<ILogger<AppRunBackgroundService>>(), p.GetRequiredService<ApplicationLifetime>());
    });
    builder.Services.AddSingleton<IMemoryUsedChecker, MemoryChecker>();
    //добавляем отправитель Email (берем данные от IOptionsSnapshot)
    builder.Services.AddScoped<IEmailSender, EmailSender>(serviceProvider =>
                    new EmailSender(serviceProvider.GetService<IOptionsSnapshot<EmailSenderConfig>>()
                    ?? throw new ArgumentNullException("IOptionsSnapshot<EmailSenderConfig>"),
                    serviceProvider.GetRequiredService<ILogger<EmailSender>>()));



    var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();


    //подстановка // вызов 
    app.MapGet("/send_email", async (IEmailSender emailSender, IMemoryUsedChecker memoryUsedChecker,
                            CancellationToken cancellationToken,
                            IOptionsSnapshot<EmailSenderConfig> options, IConfiguration config, ILogger <Program> logger) =>
    {
        int retryCountMax = 1;
        while (retryCountMax <= 3)
        {
            try
            {
                logger.LogInformation($"Sending message. Try number:  {retryCountMax}");
                await emailSender.SendEmailAsync(
                   "Kornilev M.",
                config.GetSection("EmailSenderConfig").GetValue<string>("Login") ?? throw new ArgumentNullException(nameof(config)),
                config.GetSection("EmailSenderConfig").GetValue<string>("Password") ?? throw new ArgumentNullException(nameof(config)),
               "Maxim", "kornilev.maxim@mail.ru", "MemoryUsed",
                options.Value.EmailText,
                   cancellationToken);
                logger.LogInformation("Message sent");
                return "Сообщение отправлено";
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Found an error: {ex}", ex);
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
            finally
            {
                retryCountMax++;
            }
        }
        logger.LogError($"Error with IEmailSender: message not delivered {emailSender}", emailSender); ;
        return "Сообщение не отправлено";

        //return для swagger
    });
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unexpected error");
}
finally
{
    Log.Information("Server shutted down");
    await Log.CloseAndFlushAsync();
}
