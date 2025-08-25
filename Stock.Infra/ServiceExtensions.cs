using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock.Domain.Interfaces;
using Stock.Infra.Data;
using Stock.Infra.MassTransit;
using Stock.Infra.Repositories;
using Stock.Infra.Repositories.UnitOfWork;

namespace Stock.Infra
{
    public static class ServiceExtensions
    {
        public static void ConfigureInfraServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
               options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IStockRepository, StockRepository>();

            services.AddMassTransitStock(configuration);
        }
    }
}
