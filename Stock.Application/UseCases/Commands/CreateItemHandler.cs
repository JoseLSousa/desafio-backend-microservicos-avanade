using MassTransit.Mediator;
using Stock.Domain.Entitites;
using Stock.Domain.Interfaces;

namespace Stock.Application.UseCases.Commands
{
    public class CreateItemHandler(IUnitOfWork unitOfWork, IStockRepository stockRepository) : MediatorRequestHandler<CreateItemCommand>
    {
        protected override async Task Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            StockItem newStockItem = new(request.Name, request.Quantity, request.Price);

            stockRepository.Create(newStockItem);

            await unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
