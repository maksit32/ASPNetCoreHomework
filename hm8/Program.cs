using hm8;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using Serilog;


Log.Logger = new LoggerConfiguration().WriteTo.Console()
                .CreateLogger();
Log.Information("Server started. Info by SerilogInfo");


try
{
    var builder = WebApplication.CreateBuilder(args);
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


    #region [Comment]
    //добавляем сервис отправляющий каждые 60 минут (выполнит ExecuteAsync())
    //builder.Services.AddHostedService(serviceProvider =>
    //    new BackgroundEmailMemodySender(serviceProvider.GetService<IEmailSender>(),
    //                                            serviceProvider.GetService<IMemoryUsedChecker>(),
    //                                            TimeSpan.FromMinutes(60)));

    //У него всегда Singleton Lifetime (backgroundService)
    #endregion


    var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();


    //подстановка
    app.MapGet("/send_email", async (IEmailSender emailSender, IMemoryUsedChecker memoryUsedChecker,
                            CancellationToken canclellationToken,
                            IOptionsSnapshot<EmailSenderConfig> options) =>
    {
        await emailSender.SendEmailAsync("Kornilev M.", "asp2022pd011@rodion-m.ru", "6WU4x2be",
                "Maxim", "kornilev.maxim@mail.ru", options.Value.EmailText,
                memoryUsedChecker.CheckMemoryApp(), canclellationToken);
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
