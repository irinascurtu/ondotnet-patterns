using Contracts.Commands;
using Contracts.Messages;
using Contracts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Ordering.Domain.Entities;

namespace Ordering.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMessageSession messageSession;

        public OrdersController(IMessageSession messageSession)
        {
            this.messageSession = messageSession;
        }


        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderModel model)
        {

            var sendOptions = new SendOptions();
           
            if (HttpContext.Request.Headers["TenantId"]=="SuperCoolTenant")
            {
                sendOptions.SetDestination("SuperCoolQueue");
            }

            await messageSession.Send(new CreateOrder
            {
                Id = model.Id,
                CustomerName = model.CustomerName,
                DeliveryInstructions = model.DeliveryInstructions,
                Email = model.Email,
                Phone = model.Phone,
                Status = model.Status,
                OrderItems = model.OrderItems

            });

            return Accepted();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            var request = new VerifyOrder() { OrderId = id };
            var response = await messageSession.Request<OrderResult>(request);

            if (response.IsValid)
            {
                return Ok();
            }

            return NotFound();
        }
    }
}