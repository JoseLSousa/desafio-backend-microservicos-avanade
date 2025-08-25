using Sales.Application.DTOs;

namespace Stock.Application.Events
{
    public sealed record CheckStockEvent(Guid SaleId, List<SaleItemDto> Items)
    {
    }
}
