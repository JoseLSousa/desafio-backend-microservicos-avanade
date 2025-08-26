using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Application.Commands;
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

            services.AddMediator(cfg =>
            {
                cfg.AddConsumers(typeof(ServiceExtensions).Assembly);
                cfg.AddConsumers(typeof(CreateSaleHandler).Assembly);
            });

            // Verify MassTransit IBus registration
            services.AddOptions<MassTransitHostOptions>()
                .Configure(options =>
                {
                    options.WaitUntilStarted = true;
                    options.StartTimeout = TimeSpan.FromSeconds(30);
                    options.StopTimeout = TimeSpan.FromSeconds(10);
                });
            services.AddControllers();
        }
    }
}
