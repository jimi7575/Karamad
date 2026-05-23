using Catalog.Application.Abstractions;
using Catalog.Domain.Interfaces;
using MediatR;

namespace Catalog.Application.Books;

public sealed record DeleteBookCommand(Guid Id) : IRequest<bool>;

public sealed class DeleteBookCommandHandler(IBookRepository repository, IBookCache cache) : IRequestHandler<DeleteBookCommand, bool>
{
    public async Task<bool> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var book = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (book is null) return false;

        repository.Remove(book);
        await repository.SaveChangesAsync(cancellationToken);
        await cache.RemoveAsync(request.Id, cancellationToken);
        return true;
    }
}
