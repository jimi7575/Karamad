using Catalog.Application.Books;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/books")]
public sealed class BooksController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BookDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetBooksQuery(), cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var book = await mediator.Send(new GetBookByIdQuery(id), cancellationToken);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<BookDto>> Create(CreateBookCommand command, CancellationToken cancellationToken)
    {
        var book = await mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BookDto>> Update(Guid id, UpdateBookRequest request, CancellationToken cancellationToken)
    {
        var book = await mediator.Send(new UpdateBookCommand(id, request.Title, request.Author, request.Stock, request.Price), cancellationToken);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await mediator.Send(new DeleteBookCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

public sealed record UpdateBookRequest(string Title, string Author, int Stock, decimal Price);
