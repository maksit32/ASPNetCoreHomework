using hm6;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);
//��������� swager, ����������� �������
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//�� ��� ����� ����� (��������� ������ ��������)
builder.Services.AddSingleton<IMemoryUsedChecker, MemoryChecker>();
//��������� ����������� Email
builder.Services.AddScoped<IEmailSender, EmailSender>(serviceProvider => new EmailSender());

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
app.MapGet("/sendemail", async (IEmailSender emailSender, IMemoryUsedChecker memoryUsedChecker, CancellationToken canclellationToken) => {
                await emailSender.SendEmailAsync("Kornilev M.", "asp2022pd011@rodion-m.ru", "6WU4x2be",
                        "Maxim", "kornilev.maxim@mail.ru", "Memory of App",
                        memoryUsedChecker.CheckMemoryApp(), false, canclellationToken);
                    if (emailSender is EmailSender) await((EmailSender)emailSender).DisposeAsync();
});
app.Run();
