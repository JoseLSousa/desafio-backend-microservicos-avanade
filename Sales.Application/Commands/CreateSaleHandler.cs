using MassTransit;
using MassTransit.Mediator;
using Sales.Application.Events;
using Sales.Domain.Entities;
using Sales.Domain.Interfaces;

namespace Sales.Application.Commands
{
    public class CreateSaleHandler(ISaleRepository saleRepository, IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint) : MediatorRequestHandler<CreateSaleCommand>
    {
        protected override async Task Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            var saleId = Guid.NewGuid();
            var saleItems = request.Items.Select(x => new SaleItem(x.ProductId, x.Quantity, saleId, x.Price)).ToList();
            var sale = new Sale(saleId, saleItems);

            saleRepository.Create(sale);
            await unitOfWork.CommitAsync(cancellationToken);

            await publishEndpoint.Publish(new CheckStockEvent(request.SaleId, request.Items), cancellationToken);
        }
    }
}
