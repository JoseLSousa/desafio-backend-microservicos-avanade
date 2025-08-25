using MassTransit.Mediator;
using Stock.Domain.Entitites;
using Stock.Domain.Interfaces;

namespace Stock.Application.UseCases.Queries
{
    public class GetItemByIdHandler(IStockRepository repository) : MediatorRequestHandler<GetItemByIdQuery, StockItem>
    {
        protected override async Task<StockItem> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
        {
            return await repository.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
