using MassTransit;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Stock.Application.UseCases.Commands;
using Stock.Infra;
using Stock.Infra.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration));

// Configure OpenTelemetry
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName: "Stock.WebAPI", serviceVersion: "1.0.0")
    .AddAttributes(new Dictionary<string, object>
    {
        ["deployment.environment"] = builder.Environment.EnvironmentName
    });

// Configure OpenTelemetry with metrics and tracing in a single registration
builder.Services.AddOpenTelemetry()
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

// Add services to the container
builder.Services.ConfigureInfraServices(builder.Configuration);

// Configure MassTransit Mediator for local request/response
builder.Services.AddMediator(cfg =>
{
    cfg.AddConsumers(typeof(Program).Assembly);
    cfg.AddConsumers(typeof(CreateItemHandler).Assembly);
});

// Verify MassTransit IBus registration
builder.Services.AddOptions<MassTransitHostOptions>()
    .Configure(options =>
    {
        options.WaitUntilStarted = true;
        options.StartTimeout = TimeSpan.FromSeconds(30);
        options.StopTimeout = TimeSpan.FromSeconds(10);
    });

// Health checks
builder.Services.AddHealthChecks();

builder.Services.AddControllers();

var app = builder.Build();
// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseAuthorization();
app.MapControllers();

// Map health check endpoint
app.MapHealthChecks("/health");

app.Run();
