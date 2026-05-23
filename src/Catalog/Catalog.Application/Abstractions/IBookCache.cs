using Catalog.Application.Books;

namespace Catalog.Application.Abstractions;

public interface IBookCache
{
    Task<BookDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task SetAsync(BookDto book, TimeSpan ttl, CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid id, CancellationToken cancellationToken = default);
}
