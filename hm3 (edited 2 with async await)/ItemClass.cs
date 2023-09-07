using System;
using System.Reflection;
using System.Text;
using System.Collections.Concurrent;

namespace hm3
{
    public class Shop
    {
        private ConcurrentDictionary<int, Item> items;

        public Shop()
        {
            items = new ConcurrentDictionary<int, Item>();
            AddItemsToShop(); //заполнение в через функцию
        }

        public Shop(ConcurrentDictionary<int, Item> items)
        {
            this.items = items;
        }
        //вызов(заполнение) в конструкторе
        public void AddItemsToShop()
        {
            items.TryAdd(0, new Item("Носки мужские", 20));
            items.TryAdd(1, new Item("Рубашка мужская", 150));
            items.TryAdd(2, new Item("Блузка женская", 200));
            items.TryAdd(3, new Item("Брюки мужские", 170));
            items.TryAdd(4, new Item("Кроссовки", 130));
        }

        //методы работы с магазином
        public async Task AddNewItemAsync(HttpContext context, string name, decimal price)
        {
            try
            {
                await Task.Run(() => { items.TryAdd(items.Count, new Item(name, price)); }); //тк id с 0
                context.Response.StatusCode = StatusCodes.Status201Created;
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
        public async Task DeleteItemByIdAsync(HttpContext context, int index)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (items.Count > index)
                    {
                        items.TryRemove(index, out _);
                        context.Response.StatusCode = StatusCodes.Status200OK;
                    }
                }
                catch (Exception)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            });
        }

        public async Task ClearItemsInShopAsync(HttpContext context)
        {
            await Task.Run(() =>
            {
                items.Clear();
                context.Response.StatusCode = StatusCodes.Status200OK;
            });
        }

        public async Task<string> GetItemByIdAsync(HttpContext context, int index)
        {
            try
            {
                if (items.Count > index)
                {
                    await Task.Run(() => context.Response.StatusCode = StatusCodes.Status200OK);
                    return items[index].ToString();
                }
            }
            catch (Exception)
            {
                await Task.Run(() => context.Response.StatusCode = StatusCodes.Status501NotImplemented);
                return $"Такого id не существует";
            }
            await Task.Run(() => context.Response.StatusCode = StatusCodes.Status400BadRequest);
            return $"Item with index {index} not found";
        }

        public async Task<string> UpdateItemInShopAsync(HttpContext context, int index, string name, decimal price)
        {
            try
            {
                if (items.Count > index)
                {
                    await Task.Run(() => items[index] = new Item(name, price));
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    return $"Объект по id {index} успешно изменен на  " + items[index].ToString();
                }
            }
            catch (Exception)
            {
                await Task.Run(() => context.Response.StatusCode = StatusCodes.Status501NotImplemented);
                return $"Unknown Error";
            }
            await Task.Run(() => context.Response.StatusCode = StatusCodes.Status400BadRequest);
            return $"Item with index {index} not found";
        }

        public async Task<string> ShowAllItemsAsync(HttpContext context)
        {
            var stringBuilder = new StringBuilder();
            await Task.Run(() =>
            {
                //в 1 список (конкатенируем в строку)
                for (var i = 0; i < items.Count; i++)
                {
                    stringBuilder.Append("ID - " + i);
                    stringBuilder.Append(items[i].ToString());
                    stringBuilder.Append('\n');
                }
                //foreach (var item in items)
                //{
                //    stringBuilder.Append(item.ToString());
                //    stringBuilder.Append('\n');
                //}
            });
            context.Response.StatusCode = StatusCodes.Status200OK;
            return stringBuilder.ToString();
        }
    }
    public class Item
    {
        private string name;
        private decimal price;

        public Item()
        {
            name = "No named";
            price = 0;
        }
        public Item(string name, decimal price)
        {
            this.name = name;
            this.price = price;
        }
        public Item(Item item)
        {
            this.name = item.name;
            this.price = item.price;
        }
        public string GetName()
        {
            return name;
        }
        public decimal GetPrice()
        {
            return price;
        }
        public void SetName(string name)
        {
            this.name = name;
        }
        public void SetPrice(decimal price)
        {
            this.price = price;
        }

        public override string ToString()
        {
            return $"{name}   --   {price}";
        }
    }
}
