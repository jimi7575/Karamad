using Catalog.Application.Abstractions;
using Catalog.Domain.Interfaces;
using Catalog.Infrastructure.Caching;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("CatalogDb")));

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis") ?? "localhost:6379"));

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IBookCache, RedisBookCache>();
        services.AddSingleton<RabbitMqConnection>();
        services.AddScoped<IStockEventPublisher, StockEventPublisher>();
        services.AddHostedService<OrderCreatedConsumer>();
        return services;
    }
}
