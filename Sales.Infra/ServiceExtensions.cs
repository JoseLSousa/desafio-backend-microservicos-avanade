using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Domain.Interfaces;
using Sales.Infra.Data.Context;
using Sales.Infra.Data.Repositories;
using Sales.Infra.Data.UnitOfWork;
using Sales.Infra.MassTransit;

namespace Sales.Infra
{
    public static class ServiceExtensions
    {
        public static void ConfigureInfraServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ISaleRepository, SaleRepository>();

            services.AddMassTransitSales(configuration);
        }
    }
}
