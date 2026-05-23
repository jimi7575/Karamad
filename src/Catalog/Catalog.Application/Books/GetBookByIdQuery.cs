using Catalog.Application.Abstractions;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Books;

public sealed record GetBookByIdQuery(Guid Id) : IRequest<BookDto?>;

public sealed class GetBookByIdQueryHandler(IBookRepository repository, IBookCache cache) : IRequestHandler<GetBookByIdQuery, BookDto?>
{
    public async Task<BookDto?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var cached = await cache.GetAsync(request.Id, cancellationToken);
        if (cached is not null) return cached;

        var book = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (book is null) return null;

        var dto = BookDto.FromEntity(book);
        await cache.SetAsync(dto, TimeSpan.FromMinutes(5), cancellationToken);
        return dto;
    }
}
