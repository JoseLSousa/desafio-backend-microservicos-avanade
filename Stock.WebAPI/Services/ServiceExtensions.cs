using MassTransit;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Stock.Application.UseCases.Commands;
using Stock.Application.UseCases.Queries;

namespace Stock.WebAPI.Services
{
    public static class ServiceExtensions
    {
        public static void ConfigureWebAPIServices(this IServiceCollection services)
        {
            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService(serviceName: "Stock.WebAPI", serviceVersion: "1.0.0");

            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService("Stock.WebAPI"))
                .WithMetrics(metrics =>
                {
                    metrics
                        .AddMeter("Microsoft.AspNetCore.Hosting")
                        .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                        .AddMeter("System.Net.Http")
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddOtlpExporter();
                })
                .WithTracing(tracing =>
                {
                    tracing
                        .SetResourceBuilder(resourceBuilder)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSource("Application")
                        .AddSource("MassTransit")
                        .AddOtlpExporter();
                });

            services.AddMediator(config =>
            {
                config.AddConsumers(typeof(ServiceExtensions).Assembly);
                config.AddConsumers(typeof(CreateItemHandler).Assembly);
                config.AddConsumers(typeof(GetItemByIdHandler).Assembly);
            });
            services.AddOptions<MassTransitHostOptions>()
                .Configure(options =>
                {
                    options.WaitUntilStarted = true;
                    options.StartTimeout = TimeSpan.FromSeconds(30);
                    options.StopTimeout = TimeSpan.FromSeconds(10);
                });

            services.AddHealthChecks();

            services.AddControllers();
            services.AddAuthentication();
            services.AddAuthorization();
        }
    }
}
