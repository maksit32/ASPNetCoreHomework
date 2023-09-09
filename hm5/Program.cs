using hm5;



var builder = WebApplication.CreateBuilder(args);
//добавляем swager, настраиваем сервисы
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//на все время жизни (проверяет память процесса)
builder.Services.AddSingleton<IMemoryUsedChecker, MemoryChecker>();
//добавляем отправитель Email
builder.Services.AddTransient<IEmailSender, EmailSender>(
    serviceProvider => new EmailSender(LoggerFactory.Create(builder =>
    {
        builder.AddSimpleConsole(i => i.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled);
    })
    .CreateLogger("Program")));
//добавляем сервис отправляющий каждые 60 минут (выполнит ExecuteAsync())
builder.Services.AddHostedService(serviceProvider =>
    new BackgroundEmailMemodySender(serviceProvider.GetService<IEmailSender>(), serviceProvider.GetService<IMemoryUsedChecker>(), TimeSpan.FromMinutes(60)));


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();



app.Run();
