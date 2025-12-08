using AutoMapper;
using Contracts.Mappings;
using Contracts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ordering.Domain.Entities;
using Ordering.Service;

namespace Ordering.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersRestController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersRestController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(Guid id, OrderModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            try
            {
                var order = model.ToOrder();
                await _orderService.UpdateOrderAsync(order);
            }
            catch
            {
                if (!await _orderService.OrderExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderModel model)
        {
            var orderToAdd = model.ToOrder();

            var createdOrder = await _orderService.AddOrderAsync(orderToAdd);
            await _orderService.SaveChangesAsync();
            return CreatedAtAction("GetOrder", new { id = createdOrder.Id }, createdOrder);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            await _orderService.DeleteOrderAsync(id);
            return NoContent();
        }
    }
}

