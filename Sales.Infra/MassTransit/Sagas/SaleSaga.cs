using MassTransit;
using Sales.Application.Events;

namespace Sales.Infra.MassTransit.Sagas
{
    public class SaleSaga : MassTransitStateMachine<SaleState>
    {
        public SaleSaga()
        {
            InstanceState(x => x.CurrentState);

            Event(() => StockChecked, x => x.CorrelateById(context => context.Message.SaleId));

            Initially(
                When(StockChecked)
                    .Then(context =>
                    {
                        context.Saga.Status = context.Message.IsAvaliable ? "Completed" : "Cancelled";
                    })
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }

        public Event<StockCheckedEvent> StockChecked { get; private set; }
    }
}

