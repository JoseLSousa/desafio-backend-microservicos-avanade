using Microsoft.EntityFrameworkCore;
using Serilog;
using Stock.Infra;
using Stock.Infra.Data;
using Stock.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration));

// Add services to the container
builder.Services.ConfigureInfraServices(builder.Configuration);
builder.Services.ConfigureWebAPIServices();

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
