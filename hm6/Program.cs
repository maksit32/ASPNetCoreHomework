using hm6;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);
//добавл€ем swager, настраиваем сервисы
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//на все врем€ жизни (провер€ет пам€ть процесса)
builder.Services.AddSingleton<IMemoryUsedChecker, MemoryChecker>();
//добавл€ем отправитель Email
builder.Services.AddScoped<IEmailSender, EmailSender>(serviceProvider => new EmailSender());

#region [Comment]
//добавл€ем сервис отправл€ющий каждые 60 минут (выполнит ExecuteAsync())
//builder.Services.AddHostedService(serviceProvider =>
//    new BackgroundEmailMemodySender(serviceProvider.GetService<IEmailSender>(),
//                                            serviceProvider.GetService<IMemoryUsedChecker>(),
//                                            TimeSpan.FromMinutes(60)));

//” него всегда Singleton Lifetime (backgroundService)
#endregion


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


//подстановка
app.MapGet("/sendemail", async (IEmailSender emailSender, IMemoryUsedChecker memoryUsedChecker, CancellationToken canclellationToken) => {
                await emailSender.SendEmailAsync("Kornilev M.", "asp2022pd011@rodion-m.ru", "6WU4x2be",
                        "Maxim", "kornilev.maxim@mail.ru", "Memory of App",
                        memoryUsedChecker.CheckMemoryApp(), false, canclellationToken);
                    if (emailSender is EmailSender) await((EmailSender)emailSender).DisposeAsync();
});
app.Run();
