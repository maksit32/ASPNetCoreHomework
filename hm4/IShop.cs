namespace hm4
{
    public interface IShop
    {
        public void CheckSalesDate();
        public void AddItemsToShop();
        public Task AddNewItemAsync(Item item);
        public Task DeleteItemByIdAsync(int index);
        public Task ClearItemsInShopAsync();
        public string GetItemById(int index);
        public string UpdateItemInShop(Item item, int index);
        public string ShowAllItems();
    }
}
