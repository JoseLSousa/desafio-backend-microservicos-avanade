using MassTransit;
using MassTransit.Mediator;
using Sales.Application.Events;
using Sales.Domain.Entities;
using Sales.Domain.Interfaces;
using System;
using System.Diagnostics;

namespace Sales.Application.Commands
{
    public class CreateSaleHandler(ISaleRepository saleRepository, IUnitOfWork unitOfWork, IPublishEndpoint publishEndpoint) 
        : MediatorRequestHandler<CreateSaleCommand>
    {
        protected override async Task Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            var saleItems = request.Items.Select(x => new SaleItem(x.ProductId, x.Quantity, request.SaleId, x.Price)).ToList();
            var sale = new Sale(request.SaleId, saleItems);

            saleRepository.Create(sale);
            await unitOfWork.CommitAsync(cancellationToken);

            try
            {
                Debug.WriteLine($"Sales PublishEndpoint available: {publishEndpoint != null}");
                
                // Create event with all necessary data
                var checkStockEvent = new CheckStockEvent(request.SaleId, request.Items);
                
                // Publish using the injected IPublishEndpoint - only once!
                Debug.WriteLine($"Publishing CheckStockEvent for sale {request.SaleId}");

                await publishEndpoint.Publish(checkStockEvent, cancellationToken);
                Debug.WriteLine($"Published CheckStockEvent for sale {request.SaleId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to publish CheckStockEvent: {ex.Message}");
                Debug.WriteLine($"Exception type: {ex.GetType().Name}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                // Don't rethrow - we want the API call to succeed even if publishing fails
            }
        }
    }
}
