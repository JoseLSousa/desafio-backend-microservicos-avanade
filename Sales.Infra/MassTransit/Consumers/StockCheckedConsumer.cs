using MassTransit;
using Sales.Application.Events;
using Sales.Domain.Entities;
using Sales.Domain.Interfaces;
using System;
using System.Diagnostics;

namespace Sales.Infra.MassTransit.Consumers
{
    public class StockCheckedConsumer : IConsumer<StockCheckedEvent>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StockCheckedConsumer(ISaleRepository saleRepository, IUnitOfWork unitOfWork)
        {
            _saleRepository = saleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<StockCheckedEvent> context)
        {
            Debug.WriteLine($"StockCheckedConsumer: Processing StockCheckedEvent for sale {context.Message.SaleId}");
            
            try
            {
                // Get the sale from the repository
                var sale = await _saleRepository.GetByIdAsync(context.Message.SaleId, context.CancellationToken);
                
                if (sale == null)
                {
                    Debug.WriteLine($"StockCheckedConsumer: Sale with ID {context.Message.SaleId} not found");
                    return;
                }
                
                // Only process the sale if it's still in pending state
                if (!sale.IsPending())
                {
                    Debug.WriteLine($"StockCheckedConsumer: Sale {context.Message.SaleId} is already in {sale.Status} state, skipping update");
                    return;
                }
                
                // Update the sale status based on stock availability
                if (context.Message.IsAvaliable)
                {
                    // Complete the sale
                    sale.Complete();
                    Debug.WriteLine($"StockCheckedConsumer: Sale {context.Message.SaleId} completed successfully");
                }
                else
                {
                    // Cancel the sale
                    sale.Cancel();
                    Debug.WriteLine($"StockCheckedConsumer: Sale {context.Message.SaleId} cancelled due to insufficient stock");
                }
                
                // Save the updated sale
                _saleRepository.Update(sale);
                await _unitOfWork.CommitAsync(context.CancellationToken);
                
                Debug.WriteLine($"StockCheckedConsumer: Sale {context.Message.SaleId} status updated to {sale.Status}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"StockCheckedConsumer: Error processing StockCheckedEvent: {ex.Message}");
                Debug.WriteLine($"StockCheckedConsumer: Exception type: {ex.GetType().Name}");
                Debug.WriteLine($"StockCheckedConsumer: Stack trace: {ex.StackTrace}");
                throw; // Let MassTransit retry
            }
        }
    }
}