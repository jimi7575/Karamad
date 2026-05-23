using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Orders;

namespace Order.API.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await mediator.Send(new GetOrdersQuery(), cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var order = await mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await mediator.Send(command, cancellationToken);
        return AcceptedAtAction(nameof(GetById), new { id = order.Id }, order);
    }
}
