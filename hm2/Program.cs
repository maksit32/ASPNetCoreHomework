using Microsoft.AspNetCore.Builder;

namespace hm2
{
    public class Program
    {
        //������� �������
        private static Shop myShop = new Shop(new List<Item> {new Item("����� �������", 20),
                                    new Item("������� �������", 150),
                                    new Item("������ �������", 200),
                                    new Item("����� �������", 180),
                                    new Item("����� ������", 55)});




        //main
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //��������� swager
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            //1.1 ���������� ���������
            app.MapPost("/items{name}/price/{price}", myShop.AddNewItem); //REST
            app.MapPost("/add_item", myShop.AddNewItem); //RPC
            ////1.2 �������� �������
            app.MapGet("/items", myShop.ShowAllItems); //REST
            app.MapGet("/items/{index}", myShop.GetItemById);//REST � ��������
            app.MapGet("/get_items", myShop.ShowAllItems); //RPC
            app.MapGet("/get_item", myShop.GetItemById); //RPC � ��������
            ////1.3 ������� �������
            app.MapDelete("/items/{index}", myShop.DeleteItemById); //REST
            app.MapPost("/delete_item", myShop.DeleteItemById); //RPC
            ////1.4 �������� ������
            app.MapPut("/items/{index}/{name}/{price}", myShop.UpdateItemInShop); //REST
            app.MapPost("/update_item", myShop.UpdateItemInShop); //RPC
            ////2 �������� ������ �������� 
            app.MapPost("/clear", myShop.ClearItemsInShop);







            app.Run();
        }
    }
}
