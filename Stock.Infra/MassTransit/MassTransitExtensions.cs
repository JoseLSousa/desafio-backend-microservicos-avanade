using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock.Infra.Messaging.Consumers;

namespace Stock.Infra.MassTransit
{
    public static class MassTransitExtensions
    {
        public static void AddMassTransitStock(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<CheckStockConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:HostName"]!,
                        "/",
                        h =>
                        {
                            h.Username(configuration["RabbitMQ:UserName"]!);
                            h.Password(configuration["RabbitMQ:Password"]!);
                        });
                    cfg.ReceiveEndpoint("stock-check-queue", e =>
                    {
                        e.ConfigureConsumer<CheckStockConsumer>(context);
                    });
                });
            });
        }
    }
}
