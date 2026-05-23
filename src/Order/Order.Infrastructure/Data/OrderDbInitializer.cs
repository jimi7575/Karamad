using Microsoft.Extensions.DependencyInjection;

namespace Order.Infrastructure.Data;

public static class OrderDbInitializer
{
    public static async Task MigrateAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

        for (var attempt = 1; attempt <= 30; attempt++)
        {
            try
            {
                await dbContext.Database.EnsureCreatedAsync();
                return;
            }
            catch when (attempt < 30)
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }
    }
}
