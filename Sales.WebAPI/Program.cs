using MassTransit;
using Sales.Application.Commands;
using Sales.Infra;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureInfraServices(builder.Configuration);

builder.Services.AddMediator(cfg =>
{
    cfg.AddConsumers(typeof(Program).Assembly);
    cfg.AddConsumers(typeof(CreateSaleHandler).Assembly);
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
