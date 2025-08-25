namespace Sales.Application.Events
{
    public sealed record StockCheckedEvent(Guid SaleId, bool IsAvaliable);
}
