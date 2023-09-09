using hm5;



var builder = WebApplication.CreateBuilder(args);
//��������� swager, ����������� �������
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//�� ��� ����� ����� (��������� ������ ��������)
builder.Services.AddSingleton<IMemoryUsedChecker, MemoryChecker>();
//��������� ����������� Email
builder.Services.AddTransient<IEmailSender, EmailSender>(
    serviceProvider => new EmailSender(LoggerFactory.Create(builder =>
    {
        builder.AddSimpleConsole(i => i.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Enabled);
    })
    .CreateLogger("Program")));
//��������� ������ ������������ ������ 60 ����� (�������� ExecuteAsync())
builder.Services.AddHostedService(serviceProvider =>
    new BackgroundEmailMemodySender(serviceProvider.GetService<IEmailSender>(), serviceProvider.GetService<IMemoryUsedChecker>(), TimeSpan.FromMinutes(60)));


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();



app.Run();
