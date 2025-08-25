using Sales.Domain.Common;

namespace Sales.Domain.Entities
{
    public class SaleItem : BaseEntity
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
        public Guid SaleId { get; init; }
        public int Price { get; private set; }
        public Sale? Sale { get; private set; }

        public SaleItem(Guid productId, int quantity, Guid saleId, int price)
        {
            ProductId = productId;
            Quantity = quantity;
            SaleId = saleId;
            Price = price;
        }
        public void UpdatePrice(int newPrice) => Price = newPrice;
    }
}
