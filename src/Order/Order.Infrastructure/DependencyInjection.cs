using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Abstractions;
using Order.Domain.Interfaces;
using Order.Infrastructure.Data;
using Order.Infrastructure.Messaging;

namespace Order.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddOrderInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("OrderDb")));

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddSingleton<RabbitMqConnection>();
        services.AddScoped<IOrderEventPublisher, OrderEventPublisher>();
        services.AddHostedService<StockResultConsumer>();
        return services;
    }
}
