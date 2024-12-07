using CqrsDemo.Application.Commands;
using CqrsDemo.Application.Queries;
using CqrsDemo.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CqrsDemo.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="command">The order creation details.</param>
        /// <returns>The created order.</returns>
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(Order), 201)]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
        {
            var createdOrder = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
        }

        /// <summary>
        /// Retrieves an order by ID.
        /// </summary>
        /// <param name="id">The ID of the order to retrieve.</param>
        /// <returns>The order details.</returns>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(typeof(Order), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _mediator.Send(new GetOrderByIdQuery { Id = id });
            if (order == null) return NotFound();
            return Ok(order);
        }

        /// <summary>
        /// Retrieves all orders.
        /// </summary>
        /// <returns>A list of all orders.</returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(IEnumerable<Order>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _mediator.Send(new GetAllOrdersQuery());
            return Ok(orders);
        }

        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="id">The ID of the order to update.</param>
        /// <param name="command">The updated order details.</param>
        /// <returns>The updated order.</returns>
        [HttpPut]
        [Route("{id:guid}")]
        [ProducesResponseType(typeof(Order), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderCommand command)
        {
            command.Id = id;
            var updatedOrder = await _mediator.Send(command);
            if (updatedOrder == null) return NotFound();
            return Ok(updatedOrder);
        }

        /// <summary>
        /// Deletes an order by ID.
        /// </summary>
        /// <param name="id">The ID of the order to delete.</param>
        /// <returns>No content.</returns>
        [HttpDelete("{id}")]
        [Route("{id:guid}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteOrderCommand { Id = id });
            return NoContent();
        }
    }
}
