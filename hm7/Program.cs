using hm7;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
//��������� swager, ����������� �������
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptions<EmailSenderConfig>()
                .BindConfiguration("EmailSenderConfig")
                .ValidateDataAnnotations()
                .ValidateOnStart();


builder.Services.AddSingleton<IMemoryUsedChecker, MemoryChecker>();
//��������� ����������� Email (����� ������ �� IOptionsSnapshot)
builder.Services.AddScoped<IEmailSender, EmailSender>(serviceProvider =>
                new EmailSender(serviceProvider.GetService<IOptionsSnapshot<EmailSenderConfig>>()
                ?? throw new ArgumentNullException("IOptionsSnapshot<EmailSenderConfig>")));


#region [Comment]
//��������� ������ ������������ ������ 60 ����� (�������� ExecuteAsync())
//builder.Services.AddHostedService(serviceProvider =>
//    new BackgroundEmailMemodySender(serviceProvider.GetService<IEmailSender>(),
//                                            serviceProvider.GetService<IMemoryUsedChecker>(),
//                                            TimeSpan.FromMinutes(60)));

//� ���� ������ Singleton Lifetime (backgroundService)
#endregion


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


//�����������
app.MapGet("/send_email", async (IEmailSender emailSender, IMemoryUsedChecker memoryUsedChecker, 
                        CancellationToken canclellationToken,
                        IOptionsSnapshot<EmailSenderConfig> options) => 
{
                await emailSender.SendEmailAsync("Kornilev M.", "asp2022pd011@rodion-m.ru", "6WU4x2be",
                        "Maxim", "kornilev.maxim@mail.ru", options.Value.EmailText,
                        memoryUsedChecker.CheckMemoryApp(), canclellationToken);
});
app.Run();
