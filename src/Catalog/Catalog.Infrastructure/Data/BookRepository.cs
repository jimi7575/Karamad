using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data;

public sealed class BookRepository(CatalogDbContext dbContext) : IBookRepository
{
    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Books.FirstOrDefaultAsync(book => book.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.Books.AsNoTracking().OrderBy(book => book.Title).ToListAsync(cancellationToken);

    public async Task AddAsync(Book book, CancellationToken cancellationToken = default)
        => await dbContext.Books.AddAsync(book, cancellationToken);

    public void Remove(Book book) => dbContext.Books.Remove(book);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
