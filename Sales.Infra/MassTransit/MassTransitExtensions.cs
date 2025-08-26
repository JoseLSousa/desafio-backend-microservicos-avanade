using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Application.Events;
using Sales.Infra.MassTransit.Consumers;
using Sales.Infra.MassTransit.Sagas;
using System;
using System.Diagnostics;

namespace Sales.Infra.MassTransit
{
    public static class MassTransitExtensions
    {
        public static void AddMassTransitSales(this IServiceCollection services, IConfiguration configuration)
        {
            // Register MassTransit with proper logging
            services.AddMassTransit(x =>
            {
                // Add saga state machine with in-memory repository
                x.AddSagaStateMachine<SaleSaga, SaleState>()
                  .InMemoryRepository();
                  
                // Register the StockCheckedConsumer that will update the Sale entity
                x.AddConsumer<StockCheckedConsumer>();

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

                    // Configure global message retry policy
                    cfg.UseMessageRetry(r => r.Intervals(100, 200, 500, 1000, 2000));
                    
                    // Add observers for message tracking
                    cfg.ConnectReceiveObserver(new ReceiveObserver());
                    cfg.ConnectPublishObserver(new PublishObserver());
                    
                    // Explicitly configure message types with exact same names as Stock service
                    cfg.Message<CheckStockEvent>(m => m.SetEntityName("stock-check"));
                    cfg.Message<StockCheckedEvent>(m => m.SetEntityName("stock-checked"));
                    
                    // Configure saga endpoint
                    cfg.ReceiveEndpoint("sale-state", e => {
                        // Configure the saga for this endpoint
                        e.ConfigureSaga<SaleState>(context);
                        
                        // Explicitly bind to stock-checked exchange to receive StockCheckedEvent
                        e.Bind("stock-checked", x => { });
                        
                        // Limit concurrency to avoid race conditions
                        e.ConcurrentMessageLimit = 8;
                        
                        // Add error handling - don't retry infinitely
                        e.UseMessageRetry(r => r.Immediate(3));
                        
                        // Enable dead-letter queue
                        e.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(3)));
                        e.UseInMemoryOutbox();
                    });
                    
                    // Add endpoint for StockCheckedConsumer
                    cfg.ReceiveEndpoint("stock-checked-consumer", e =>
                    {
                        e.ConfigureConsumer<StockCheckedConsumer>(context);
                        e.Bind("stock-checked", x => { });
                        e.UseMessageRetry(r => r.Immediate(3));
                    });
                    
                    // Configure remaining endpoints based on conventions
                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }

    // Observer for receive operations
    public class ReceiveObserver : IReceiveObserver
    {
        public Task PreReceive(ReceiveContext context) 
        {
            Debug.WriteLine($"About to receive message from {context.InputAddress}");
            return Task.CompletedTask;
        }

        public Task PostReceive(ReceiveContext context) 
        {
            Debug.WriteLine($"Received message from {context.InputAddress}");
            return Task.CompletedTask;
        }

        public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan consumeTime, string consumerType) where T : class
        {
            Debug.WriteLine($"Message {typeof(T).Name} consumed by {consumerType} in {consumeTime.TotalMilliseconds}ms");
            return Task.CompletedTask;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan consumeTime, string consumerType, Exception exception) where T : class
        {
            Debug.WriteLine($"Error consuming {typeof(T).Name} by {consumerType}: {exception.Message}");
            return Task.CompletedTask;
        }

        public Task ReceiveFault(ReceiveContext context, Exception exception)
        {
            Debug.WriteLine($"Error receiving message: {exception.Message}");
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
