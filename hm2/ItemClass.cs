using System;
using System.Reflection;
using System.Text;

namespace hm2
{
    public class Shop
    {
        private List<Item> items;

        public Shop()
        {
            items = new List<Item>();
        }

        public Shop(List<Item> items)
        {
            this.items = items;
        }

        public Shop(Item[] itemsArray)
        {
            this.items = new List<Item>(itemsArray);
        }

        //методы работы с магазином
        //public void AddNewItem(HttpContext context, Item item)
        //{
        //    try
        //    {
        //        items.Add(item);
        //        context.Response.StatusCode = StatusCodes.Status201Created;
        //    }
        //    catch (Exception)
        //    {
        //        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        //    }
        //}
        public void AddNewItem(HttpContext context, string name, decimal price)
        {
            try
            {
                items.Add(new Item(name, price));
                context.Response.StatusCode = StatusCodes.Status201Created;
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }        
        }
        public void DeleteItemById(HttpContext context, int index)
        {
            try
            {
                if (items.Count > index)
                {
                    items.RemoveAt(index);
                }
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        public void ClearItemsInShop(HttpContext context)
        {
            items.Clear();
            context.Response.StatusCode = StatusCodes.Status200OK;
        }

        public string GetItemById(HttpContext context, int index)
        {
            try
            {
                if (items.Count > index)
                {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    return items[index].ToString();
                }
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status501NotImplemented;
                return $"Unknown Error";
            }
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return $"Item with index {index} not found";
        }

        public string UpdateItemInShop(HttpContext context, int index, string name, decimal price)
        {
            try
            {
                if (items.Count > index)
                {
                    items[index] = new Item(name, price);
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    return $"Объект по id {index} успешно изменен на  " + items[index].ToString();
                }
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status501NotImplemented;
                return $"Unknown Error";
            }
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return $"Item with index {index} not found";
        }

        public string ShowAllItems(HttpContext context)
        {
            var stringBuilder = new StringBuilder();
            //в 1 список (конкатенируем в строку)
            foreach (var item in items)
            {
                stringBuilder.Append(item.ToString());
                stringBuilder.Append('\n');
            }
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
