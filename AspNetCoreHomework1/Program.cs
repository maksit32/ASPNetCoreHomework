using AspNetCoreHomework1;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();





//1
app.MapGet("/customs_duty", (float price) => $"���� ������� ��� ���������� 200 ����: {FunctionsClass.Duty(price)}");


//2
app.MapGet("/get_full_date", (string language) => FunctionsClass.GetDate(language));


app.Run();




