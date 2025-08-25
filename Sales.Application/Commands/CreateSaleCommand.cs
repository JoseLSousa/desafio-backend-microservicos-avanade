using Sales.Application.DTOs;

namespace Sales.Application.Commands
{
    public sealed record CreateSaleCommand(Guid SaleId, List<SaleItemDto> Items);
}
