using Microsoft.AspNetCore.Builder;

namespace hm2
{
    public class Program
    {
        //создали магазин
        private static Shop myShop = new Shop(new List<Item> {new Item("Носки мужские", 20),
                                    new Item("Рубашка мужская", 150),
                                    new Item("Блузка женская", 200),
                                    new Item("Брюки мужские", 180),
                                    new Item("Шорты летние", 55)});




        //main
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();



            //1.1 добавление элементов
            app.MapPost("/items{name}/price/{price}", myShop.AddNewItem); //REST
            app.MapPost("/add_item", myShop.AddNewItem); //RPC
            ////1.2 получить элемент
            app.MapGet("/items", myShop.ShowAllItems); //REST
            app.MapGet("/items/{index}", myShop.GetItemById);//REST с индексом
            app.MapGet("/get_items", myShop.ShowAllItems); //RPC
            app.MapGet("/get_item", myShop.GetItemById); //RPC с индексом
            ////1.3 удалить элемент
            app.MapDelete("/items/{index}", myShop.DeleteItemById); //REST
            app.MapPost("/delete_item", myShop.DeleteItemById); //RPC
            ////1.4 изменить объект
            app.MapPut("/items/{index}/{name}/{price}", myShop.UpdateItemInShop); //REST
            app.MapPost("/update_item", myShop.UpdateItemInShop); //RPC
            ////2 очистить список магазина 
            app.MapPost("/clear", myShop.ClearItemsInShop);







            app.Run();
        }
    }
}
