using MassTransit;
using Sales.Application.Events;
using Stock.Application.Events;
using Stock.Domain.Interfaces;

namespace Stock.Infra.Messaging.Consumers
{
    public class CheckStockConsumer(IStockRepository stockRepository, IPublishEndpoint publishEndpoint) : IConsumer<CheckStockEvent>
    {
        public async Task Consume(ConsumeContext<CheckStockEvent> context)
        {
            bool available = true;
            foreach (var item in context.Message.Items)
            {
                var stockItem = await stockRepository.GetByIdAsync(item.ProductId, context.CancellationToken);
                if (stockItem == null || stockItem.Quantity < item.Quantity)
                {
                    available = false;
                    break;
                }
            }
            await publishEndpoint.Publish(new StockCheckedEvent(context.Message.SaleId, available), context.CancellationToken);
        }
    }
}
