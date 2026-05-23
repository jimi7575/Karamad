using Microsoft.EntityFrameworkCore;
using Order.Domain.Entities;

namespace Order.Infrastructure.Data;

public sealed class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<CustomerOrder> Orders => Set<CustomerOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerOrder>(entity =>
        {
            entity.HasKey(order => order.Id);
            entity.Property(order => order.Status).HasConversion<string>().HasMaxLength(32);
            entity.Property(order => order.FailureReason).HasMaxLength(500);
        });
    }
}
