using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Infra.MassTransit.Sagas;

namespace Sales.Infra.MassTransit
{
    public static class MassTransitExtensions
    {
        public static void AddMassTransitSales(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<SaleSaga, SaleState>()
                .InMemoryRepository();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:HostName"],
                        "/",
                        h =>
                        {
                            h.Username(configuration["RabbitMQ:UserName"]!);
                            h.Password(configuration["RabbitMQ:Password"]!);
                        });
                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}
