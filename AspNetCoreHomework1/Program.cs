using AspNetCoreHomework1;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();





//1
app.MapGet("/customs_duty", (float price) => $"���� ������� ��� ���������� 200 ����: {FunctionsClass.Duty(price)}");

//https://localhost:7205/get_full_date?language=ru
//2
app.MapGet("/get_full_date", (string language) => FunctionsClass.GetDate(language));


app.Run();




