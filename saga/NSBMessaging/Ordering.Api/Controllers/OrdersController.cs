using Contracts.Commands;
using Contracts.Messages;
using Contracts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Ordering.Domain.Entities;
using Ordering.Service;

namespace Ordering.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMessageSession messageSession;
        private readonly IOrderIntakeService orderIntakeService;

        public OrdersController(IMessageSession messageSession, IOrderIntakeService orderIntakeService)
        {
            this.messageSession = messageSession;
            this.orderIntakeService = orderIntakeService;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderModel model)
        {
            await orderIntakeService.AcceptOrder(model);
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