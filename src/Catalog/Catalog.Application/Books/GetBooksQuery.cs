using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Books;

public sealed record GetBooksQuery : IRequest<IReadOnlyList<BookDto>>;

public sealed class GetBooksQueryHandler(IBookRepository repository) : IRequestHandler<GetBooksQuery, IReadOnlyList<BookDto>>
{
    public async Task<IReadOnlyList<BookDto>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        var books = await repository.GetAllAsync(cancellationToken);
        return books.Select(BookDto.FromEntity).ToList();
    }
}
