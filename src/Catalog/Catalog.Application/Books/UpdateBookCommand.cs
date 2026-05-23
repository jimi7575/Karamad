using Catalog.Application.Abstractions;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Books;

public sealed record UpdateBookCommand(Guid Id, string Title, string Author, int Stock, decimal Price) : IRequest<BookDto?>;

public sealed class UpdateBookCommandHandler(IBookRepository repository, IBookCache cache) : IRequestHandler<UpdateBookCommand, BookDto?>
{
    public async Task<BookDto?> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (book is null) return null;

        book.Update(request.Title, request.Author, request.Stock, request.Price);
        await repository.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(request.Id, cancellationToken);
        return BookDto.FromEntity(book);
    }
}
