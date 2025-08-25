using Sales.Domain.Common;

namespace Sales.Domain.Entities
{
    public class Sale : BaseEntity
    {
        public ICollection<SaleItem> Items { get; private set; }
        public SaleStatus Status { get; private set; } = SaleStatus.Pending;

        public Sale(Guid id, List<SaleItem> items)
        {
            Id = id;
            Items = items;
        }

        // Parameterless constructor for EF Core
        protected Sale()
        {
            Items = new List<SaleItem>();
        }
    }

    public enum SaleStatus
    {
        Pending,
        Completed,
        Cancelled
    }
}
