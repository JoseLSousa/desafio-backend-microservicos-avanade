using MassTransit;
using Sales.Application.Events;
using System;
using System.Diagnostics;

namespace Sales.Infra.MassTransit.Sagas
{
    public class SaleSaga : MassTransitStateMachine<SaleState>
    {
        public SaleSaga()
        {
            InstanceState(x => x.CurrentState);

            // Configure event correlation - use the SaleId as the correlation ID
            Event(() => StockChecked, x => 
                x.CorrelateById(context => context.Message.SaleId));

            // Define the Initial state behavior (when saga instance is created or found)
            Initially(
                When(StockChecked)
                    .Then(context =>
                    {
                        Debug.WriteLine($"Initial state: Processing StockCheckedEvent for sale {context.Message.SaleId}, available: {context.Message.IsAvaliable}");
                        
                        // Update saga state based on stock check result
                        context.Saga.Status = context.Message.IsAvaliable ? "Completed" : "Cancelled";
                        context.Saga.Updated = DateTime.UtcNow;
                        
                        Debug.WriteLine($"Initial state: Sale {context.Message.SaleId} status updated to: {context.Saga.Status}");
                    })
                    .TransitionTo(Processed)
            );

            // Define states
            State(() => Processed);

            // Handle duplicate events in the Processed state
            During(Processed,
                When(StockChecked)
                    .Then(context =>
                    {
                        Debug.WriteLine($"Processed state: Received duplicate StockCheckedEvent for sale {context.Message.SaleId}");
                        // Just log, don't change state
                    })
            );

            // Add this to handle events in the Final state (to ignore duplicates)
            DuringAny(
                When(StockChecked)
                    .Then(context =>
                    {
                        Debug.WriteLine($"Any state: Received StockCheckedEvent for sale {context.Message.SaleId} in state {context.Saga.CurrentState}");
                    })
            );
        }

        // Define states
        public State Processed { get; private set; }

        // Event definitions
        public Event<StockCheckedEvent> StockChecked { get; private set; }
    }
}

