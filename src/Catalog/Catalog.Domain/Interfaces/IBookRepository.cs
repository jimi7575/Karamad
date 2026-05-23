using Catalog.Domain.Entities;

namespace Catalog.Domain.Interfaces;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Book book, CancellationToken cancellationToken = default);
    void Remove(Book book);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
