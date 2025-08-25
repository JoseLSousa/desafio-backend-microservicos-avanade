using MassTransit;

namespace Sales.Infra.MassTransit.Sagas
{
    public class SaleState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
