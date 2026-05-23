using Order.Domain.Entities;

namespace Order.Domain.Interfaces;

public interface IOrderRepository
{
    Task<CustomerOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CustomerOrder>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(CustomerOrder order, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
