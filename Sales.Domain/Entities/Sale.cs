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
        
        // Update the status with a specific status
        public void UpdateStatus(SaleStatus newStatus)
        {
            Status = newStatus;
            MarkAsUpdated();
        }
        
        // Complete the sale
        public void Complete()
        {
            if (Status != SaleStatus.Pending)
                throw new InvalidOperationException($"Cannot complete sale with status {Status}");
                
            Status = SaleStatus.Completed;
            MarkAsUpdated();
        }
        
        // Cancel the sale
        public void Cancel()
        {
            if (Status != SaleStatus.Pending)
                throw new InvalidOperationException($"Cannot cancel sale with status {Status}");
                
            Status = SaleStatus.Cancelled;
            MarkAsUpdated();
        }
        
        // Calculate the total value of the sale
        public decimal GetTotal()
        {
            return Items.Sum(item => item.Price * item.Quantity);
        }
        
        // Check if the sale is in a specific status
        public bool IsInStatus(SaleStatus status)
        {
            return Status == status;
        }
        
        // Check if the sale is pending
        public bool IsPending() => Status == SaleStatus.Pending;
        
        // Check if the sale is completed
        public bool IsCompleted() => Status == SaleStatus.Completed;
        
        // Check if the sale is cancelled
        public bool IsCancelled() => Status == SaleStatus.Cancelled;
    }

    public enum SaleStatus
    {
        Pending,
        Completed,
        Cancelled
    }
}
