using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Application.Events;
using Stock.Application.Events;
using Stock.Infra.Messaging.Consumers;
using System.Diagnostics;

namespace Stock.Infra.MassTransit
{
    public static class MassTransitExtensions
    {
        public static void AddMassTransitStock(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                // Register consumers
                x.AddConsumer<CheckStockConsumer>();

                // Configure RabbitMQ
                x.UsingRabbitMq((context, cfg) =>
                {
                    var hostName = configuration["RabbitMQ:HostName"];
                    var userName = configuration["RabbitMQ:UserName"];
                    var password = configuration["RabbitMQ:Password"];

                    // Ensure we have valid configuration
                    if (string.IsNullOrEmpty(hostName))
                    {
                        throw new InvalidOperationException("RabbitMQ:HostName configuration is missing");
                    }

                    cfg.Host(hostName, "/", h =>
                    {
                        h.Username(userName ?? "guest");
                        h.Password(password ?? "guest");
                    });

                    // IMPORTANT: Use the same name as the exchange defined in the Sales service
                    cfg.ReceiveEndpoint("stock-check", e =>
                    {
                        e.ConfigureConsumer<CheckStockConsumer>(context);
                        e.UseMessageRetry(r => r.Intervals(100, 500, 1000));

                        // Critical: Add binding directly to the queue
                        e.Bind("stock-check", x => { });

                        // Set prefetch count to ensure messages are processed one at a time
                        e.PrefetchCount = 16;

                        // Add error handling
                        e.UseMessageRetry(r => r.Immediate(5));

                        // Configure concurrent message limit
                        e.ConcurrentMessageLimit = 1;
                    });

                    // Add observers for message tracking
                    cfg.ConnectSendObserver(new SendObserver());
                    cfg.ConnectPublishObserver(new PublishObserver());

                    // Explicitly configure message types with proper exchange names
                    cfg.Message<ItemCreatedEvent>(m => m.SetEntityName("stock-item-created"));
                    cfg.Message<CheckStockEvent>(m => m.SetEntityName("stock-check"));
                    cfg.Message<StockCheckedEvent>(m => m.SetEntityName("stock-checked"));

                    // Configure endpoints based on conventions
                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }

    // Observer for send operations
    public class SendObserver : ISendObserver
    {
        public Task PreSend<T>(SendContext<T> context) where T : class
        {
            Debug.WriteLine($"About to send message of type {typeof(T).Name} to {context.DestinationAddress}");
            return Task.CompletedTask;
        }

        public Task PostSend<T>(SendContext<T> context) where T : class
        {
            Debug.WriteLine($"Sent message of type {typeof(T).Name} to {context.DestinationAddress}");
            return Task.CompletedTask;
        }

        public Task SendFault<T>(SendContext<T> context, Exception exception) where T : class
        {
            Debug.WriteLine($"Error sending {typeof(T).Name} to {context.DestinationAddress}: {exception.Message}");
            return Task.CompletedTask;
        }
    }

    // Observer for publish operations
    public class PublishObserver : IPublishObserver
    {
        public Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            Debug.WriteLine($"About to publish message of type {typeof(T).Name}");
            return Task.CompletedTask;
        }

        public Task PostPublish<T>(PublishContext<T> context) where T : class
        {
            Debug.WriteLine($"Published message of type {typeof(T).Name}");
            return Task.CompletedTask;
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
        {
            Debug.WriteLine($"Error publishing {typeof(T).Name}: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}
