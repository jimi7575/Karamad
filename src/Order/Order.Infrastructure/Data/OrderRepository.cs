using Microsoft.EntityFrameworkCore;
using Order.Domain.Entities;
using Order.Domain.Interfaces;

namespace Order.Infrastructure.Data;

public sealed class OrderRepository(OrderDbContext dbContext) : IOrderRepository
{
    public async Task<CustomerOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Orders.FirstOrDefaultAsync(order => order.Id == id, cancellationToken);

    public async Task<IReadOnlyList<CustomerOrder>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.Orders.AsNoTracking().OrderByDescending(order => order.CreatedAtUtc).ToListAsync(cancellationToken);

    public async Task AddAsync(CustomerOrder order, CancellationToken cancellationToken = default)
        => await dbContext.Orders.AddAsync(order, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
