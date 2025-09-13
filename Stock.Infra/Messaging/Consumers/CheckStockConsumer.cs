using MassTransit;
using Sales.Application.Events;
using Stock.Domain.Entitites;
using Stock.Domain.Interfaces;
using System.Diagnostics;

namespace Stock.Infra.Messaging.Consumers
{
    public class CheckStockConsumer : IConsumer<CheckStockEvent>
    {
        private readonly IStockRepository _stockRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IUnitOfWork _unitOfWork;

        public CheckStockConsumer(IStockRepository stockRepository, IPublishEndpoint publishEndpoint, IUnitOfWork unitOfWork)
        {
            _stockRepository = stockRepository;
            _publishEndpoint = publishEndpoint;
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<CheckStockEvent> context)
        {
            Debug.WriteLine($"Consuming CheckStockEvent for sale {context.Message.SaleId}");

            try
            {
                // Dictionary to store items and their quantities for updating if available
                var itemsToUpdate = new Dictionary<StockItem, int>();
                bool available = true;

                // Check if all items are available
                foreach (var item in context.Message.Items)
                {
                    var stockItem = await _stockRepository.GetByIdAsync(item.ProductId, context.CancellationToken);
                    if (stockItem == null || stockItem.Quantity < item.Quantity)
                    {
                        available = false;
                        Debug.WriteLine($"Item {item.ProductId} not available in sufficient quantity. Required: {item.Quantity}, Available: {stockItem?.Quantity ?? 0}");
                        break;
                    }

                    // Add to items to update if available
                    itemsToUpdate[stockItem] = item.Quantity;
                }

                // If all items are available, reduce stock quantities
                if (available)
                {
                    Debug.WriteLine("All items are available, reducing stock quantities...");

                    // Update each stock item by reducing its quantity
                    foreach (var kvp in itemsToUpdate)
                    {
                        var stockItem = kvp.Key;
                        var quantityToDeduct = kvp.Value;

                        Debug.WriteLine($"Reducing stock for item {stockItem.Id} ({stockItem.Name}) by {quantityToDeduct}. " +
                                      $"Current quantity: {stockItem.Quantity}");

                        // Deduct the quantity
                        stockItem.Deduct(quantityToDeduct);

                        // Update in repository
                        _stockRepository.Update(stockItem);

                        Debug.WriteLine($"New quantity for item {stockItem.Id}: {stockItem.Quantity}");
                    }

                    // Save changes to database
                    await _unitOfWork.CommitAsync(context.CancellationToken);
                    Debug.WriteLine("Stock quantities updated successfully");
                }

                // Create the response event
                var stockCheckedEvent = new StockCheckedEvent(context.Message.SaleId, available);

                Debug.WriteLine($"Created StockCheckedEvent for sale {context.Message.SaleId} with available={available}");

                // Only publish once - use ConsumeContext to publish
                // This ensures the message is published in the same transaction
                await context.Publish(stockCheckedEvent);

                Debug.WriteLine($"Successfully published StockCheckedEvent for sale {context.Message.SaleId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error processing CheckStockEvent: {ex.Message}");
                Debug.WriteLine($"Exception type: {ex.GetType().Name}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                // Re-throw to let MassTransit retry policy handle it
                throw;
            }
        }
    }
}
