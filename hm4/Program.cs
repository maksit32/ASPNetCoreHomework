using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace hm4
{
    public class Program
    {
        //������� ������� + ��������� � ������������
        //private static IShop myShop = new Shop(new UTCClock());




        //main
        public static void Main(string[] args)
        {
            //���������
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            //��������� swager
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IShop, Shop>(); //�������� Singleton (�������� �������)
            //��������
            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });


            //1.1 ���������� ���������
            //��������! ���������� Singleton + �������� ������� + �������� DIP
            app.MapPost("/items", ([FromServices] IShop myShop, [FromBody] Item item, HttpContext context) =>
                                {
                                    myShop.AddNewItemAsync(item);
                                    context.Response.StatusCode = StatusCodes.Status201Created;
                                }); //REST
            app.MapPost("/add_item", ([FromServices] IShop myShop, [FromBody] Item item, HttpContext context) =>
                                {
                                    myShop.AddNewItemAsync(item);
                                    context.Response.StatusCode = StatusCodes.Status201Created;
                                }); //RPC
            //1.2 �������� �������
            app.MapGet("/items", ([FromServices] IShop myShop) =>
                                {
                                    return myShop.ShowAllItems();
                                }); //REST
            app.MapGet("/items/{index:int}", ([FromServices] IShop myShop, int index) =>
                                {
                                    return myShop.GetItemById(index);
                                });//REST � ��������
            app.MapGet("/get_items", ([FromServices] IShop myShop) =>
                                {
                                    return myShop.ShowAllItems();
                                }); //RPC
            app.MapGet("/get_item", ([FromServices] IShop myShop, [FromQuery] int index) =>
                                {
                                    return myShop.GetItemById(index);
                                }); //RPC � ��������
            //1.3 ������� �������
            app.MapDelete("/items/{index:int}", ([FromServices] IShop myShop, int index, HttpContext context) =>
                                {
                                    myShop.DeleteItemByIdAsync(index);
                                    context.Response.StatusCode = StatusCodes.Status200OK;
                                }); //REST
            app.MapPost("/delete_item", ([FromServices] IShop myShop, [FromQuery] int index, HttpContext context) =>
                                {
                                    myShop.DeleteItemByIdAsync(index);
                                    context.Response.StatusCode = StatusCodes.Status200OK;
                                }); //RPC
            //1.4 �������� ������
            app.MapPut("/items/{index:int}", ([FromServices] IShop myShop, [FromBody] Item item, int index, HttpContext context) =>
                                {
                                    myShop.UpdateItemInShop(item, index);
                                    context.Response.StatusCode = StatusCodes.Status200OK;
                                }); //REST
            app.MapPost("/update_item", ([FromServices] IShop myShop, [FromBody] Item item, [FromQuery] int index, HttpContext context) =>
                                {
                                    myShop.UpdateItemInShop(item, index);
                                    context.Response.StatusCode = StatusCodes.Status200OK;
                                }); //RPC
            //2 �������� ������ �������� 
            app.MapPost("/clear", ([FromServices] IShop myShop, HttpContext context) =>
                                {
                                    myShop.ClearItemsInShopAsync();
                                    context.Response.StatusCode = StatusCodes.Status200OK;
                                });
            app.MapGet("/checksalesdate", ([FromServices] IShop myShop) => 
            {
                myShop.CheckSalesDate();
            });

            //FromServices - �� �������� (Singleton - ��������)
            //FromQuery - �� ������ �������
            //FromBody - �� ������� (�������� json ���)



            app.Run();
        }
    }
}
