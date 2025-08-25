using MassTransit.Mediator;
using Stock.Domain.Entitites;

namespace Stock.Application.UseCases.Queries
{
    public sealed record GetItemByIdQuery(Guid Id) : Request<StockItem>;
}
