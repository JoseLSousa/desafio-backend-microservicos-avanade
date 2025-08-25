using Sales.Application.DTOs;

namespace Sales.Application.Events
{
    public sealed record CheckStockEvent(Guid SaleId, List<SaleItemDto> Items)
    {
    }
}
