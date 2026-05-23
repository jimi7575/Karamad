using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Books;

public sealed record CreateBookCommand(string Title, string Author, int Stock, decimal Price) : IRequest<BookDto>;

public sealed class CreateBookCommandHandler(IBookRepository repository) : IRequestHandler<CreateBookCommand, BookDto>
{
    public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var book = new Book(request.Title, request.Author, request.Stock, request.Price);
        await repository.AddAsync(book, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        return BookDto.FromEntity(book);
    }
}
