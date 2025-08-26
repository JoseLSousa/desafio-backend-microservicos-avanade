using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Sales.Infra;
using Sales.Infra.Data.Context;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration));

// Configure OpenTelemetry
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName: "Sales.WebAPI", serviceVersion: "1.0.0")
    .AddAttributes(new Dictionary<string, object>
    {
        ["deployment.environment"] = builder.Environment.EnvironmentName
    });

// Configure OpenTelemetry with metrics and tracing in a single registration
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Sales.WebAPI"))
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

// Health checks
builder.Services.AddHealthChecks();

// Add services to the container
builder.Services.ConfigureInfraServices(builder.Configuration);

var app = builder.Build();

// Health endpoint
app.MapHealthChecks("/health");

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>()
        .Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseAuthorization();
app.MapControllers();

app.Run();
