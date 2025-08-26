namespace Stock.Application.Events
{
    public sealed record ItemCreatedEvent(Guid ItemId, string Name, int Quantity, int Price);
}