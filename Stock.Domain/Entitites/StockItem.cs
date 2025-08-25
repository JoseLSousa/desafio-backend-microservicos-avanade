using Stock.Domain.Common;

namespace Stock.Domain.Entitites
{
    public class StockItem : BaseEntity
    {
        public string Name { get; private set; }
        public int Quantity { get; private set; }
        public int Price { get; private set; }

        public StockItem(string name, int quantity, int price)
        {
            Quantity = quantity;
            Name = name;
            Price = price;
        }

        public void Deduct(int amount) => Quantity -= amount;
        public void Replenish(int amount) => Quantity += amount;
        public void UpdatePrice(int newPrice) => Price = newPrice;
        public void UpdateName(string newName) => Name = newName;
    }
}
