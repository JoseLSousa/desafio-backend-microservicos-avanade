namespace Sales.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; private set; }
        public DateTimeOffset? DeletedAt { get; private set; }
        public bool IsDeleted { get; private set; } = false;

        public void MarkAsUpdated()
        {
            UpdatedAt = DateTimeOffset.UtcNow;
        }
    }
}
