using Microsoft.AspNetCore.Builder;
using System.Collections.Concurrent;

namespace hm3
{
    public class Program
    {
        //создали магазин + заполнили в конструкторе
        private static Shop myShop = new Shop();




        //main
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //добавляем swager
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            //1.1 добавление элементов
            app.MapPost("/items{name}/price/{price}", myShop.AddNewItemAsync); //REST
            app.MapPost("/add_item", myShop.AddNewItemAsync); //RPC
            ////1.2 получить элемент
            app.MapGet("/items", myShop.ShowAllItemsAsync); //REST
            app.MapGet("/items/{index}", myShop.GetItemByIdAsync);//REST с индексом
            app.MapGet("/get_items", myShop.ShowAllItemsAsync); //RPC
            app.MapGet("/get_item", myShop.GetItemByIdAsync); //RPC с индексом
            ////1.3 удалить элемент
            app.MapDelete("/items/{index}", myShop.DeleteItemByIdAsync); //REST
            app.MapPost("/delete_item", myShop.DeleteItemByIdAsync); //RPC
            ////1.4 изменить объект
            app.MapPut("/items/{index}/{name}/{price}", myShop.UpdateItemInShopAsync); //REST
            app.MapPost("/update_item", myShop.UpdateItemInShopAsync); //RPC
            ////2 очистить список магазина 
            app.MapPost("/clear", myShop.ClearItemsInShopAsync);







            app.Run();
        }
    }
}
