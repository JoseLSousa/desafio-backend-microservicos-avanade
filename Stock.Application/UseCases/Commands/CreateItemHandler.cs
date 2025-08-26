using MassTransit;
using MassTransit.Mediator;
using Stock.Application.Events;
using Stock.Domain.Entitites;
using Stock.Domain.Interfaces;
using System.Diagnostics;

namespace Stock.Application.UseCases.Commands
{
    public class CreateItemHandler(IUnitOfWork unitOfWork, IStockRepository stockRepository, IPublishEndpoint publishEndpoint, IBus bus) : MediatorRequestHandler<CreateItemCommand>
    {
        protected override async Task Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            StockItem newStockItem = new(request.Name, request.Quantity, request.Price);

            stockRepository.Create(newStockItem);

            await unitOfWork.CommitAsync(cancellationToken);

            try
            {
                // Log the bus and endpoint availability
                Debug.WriteLine($"Bus available: {bus != null}");
                Debug.WriteLine($"PublishEndpoint available: {publishEndpoint != null}");
                
                // Create event with all necessary data
                var itemCreatedEvent = new ItemCreatedEvent(newStockItem.Id, request.Name, request.Quantity, request.Price);
                
                // Publish using the injected IPublishEndpoint
                Debug.WriteLine("Attempting to publish ItemCreatedEvent using IPublishEndpoint...");
                if (publishEndpoint != null)
                {
                    await publishEndpoint.Publish(itemCreatedEvent, cancellationToken);
                    Debug.WriteLine($"Published ItemCreatedEvent for item {newStockItem.Id} using IPublishEndpoint");
                }
                
                // Publish directly using IBus
                Debug.WriteLine("Attempting to publish ItemCreatedEvent using IBus...");
                if (bus != null) 
                {
                    await bus.Publish(itemCreatedEvent, cancellationToken);
                    Debug.WriteLine($"Published ItemCreatedEvent for item {newStockItem.Id} using IBus");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to publish ItemCreatedEvent: {ex.Message}");
                Debug.WriteLine($"Exception type: {ex.GetType().Name}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
