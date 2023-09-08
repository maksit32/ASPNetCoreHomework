using System;
using System.Reflection;
using System.Text;
using System.Collections.Concurrent;

namespace hm4
{
    public class Shop : IShop
    {
        private ConcurrentDictionary<int, Item> items;
        private IClock clock;
        private bool changedDayOfWeek = false;
        //дефолтный конструктор
        public Shop()
        {
            items = new ConcurrentDictionary<int, Item>();
            clock = new UTCClock(); //по дефолту UTC
            AddItemsToShop(); //заполнение в через функцию
            CheckSalesDate();
        }
        //с DIP
        public Shop(IClock iclock)
        {
            items = new ConcurrentDictionary<int, Item>();
            AddItemsToShop();
            this.clock = iclock;
            CheckSalesDate();
        }
        //DIP
        public Shop(ConcurrentDictionary<int, Item> items, IClock clock)
        {
            this.items = items;
            this.clock = clock;
            CheckSalesDate();
        }
        public void CheckSalesDate()
        {
            //если понедельник, то меняем цены со скидкой
            if (clock.ShowClock().DayOfWeek == DayOfWeek.Monday && !changedDayOfWeek)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].SetPrice(items[i].GetPrice() * 0.7m);
                }
                changedDayOfWeek = true;
            }
            else if (!(clock.ShowClock().DayOfWeek == DayOfWeek.Monday) && changedDayOfWeek)//вернем цены обратно
            {
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].SetPrice(items[i].GetPrice() * 100 / 70);
                }
                changedDayOfWeek = false;
            }
        }
        public bool GetChangedDayOfWeek()
        {
            return changedDayOfWeek;
        }
        public void SetChangedDayOfWeek(bool changed)
        {
            changedDayOfWeek = changed;
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

        public async Task DeleteItemByIdAsync(int index)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (items.Count > index)
                    {
                        items.TryRemove(index, out _);
                    }
                }
                catch (Exception)
                {
  
                }
            });
        }

        public async Task ClearItemsInShopAsync()
        {
            await Task.Run(() =>
            {
                items.Clear();
            });
        }

        public string GetItemById(int index)
        {
            try
            {
                if (items.Count > index)
                {
                    return items[index].ToString();
                }
            }
            catch (Exception)
            {
                return $"Такого id не существует";
            }
            return $"Item with index {index} not found";
        }

        public string UpdateItemInShop(Item item, int index)
        {
            try
            {
                if (items.Count > index)
                {
                    items[index] = item;
                    return $"Объект по id {index} успешно изменен на  " + items[index].ToString();
                }
            }
            catch (Exception)
            {
                return $"Unknown Error";
            }
            return $"Item with index {index} not found";
        }

        public string ShowAllItems()
        {
            var stringBuilder = new StringBuilder();
            //в 1 список (конкатенируем в строку)
            for (var i = 0; i < items.Count; i++)
            {
                stringBuilder.Append("ID - " + i + " ");
                stringBuilder.Append(items[i].ToString());
                stringBuilder.Append('\n');
            }
            return stringBuilder.ToString();
        }

        public async Task AddNewItemAsync(Item item)
        {
            items.TryAdd(items.Count, item);
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
