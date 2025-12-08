using Contracts.Events;
using Microsoft.AspNetCore.Mvc;
using PaymentProcessor.Models;

namespace PaymentProcessor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMessageSession messageSession;

        public PaymentController(IMessageSession messageSession)
        {
            this.messageSession = messageSession;
        }

        [HttpPost("/pay")]
        public async Task<IActionResult> Post(PayModel model)
        {
            //do some work and, actually capture the payment
            //it might succeed or fail

            await messageSession.Publish(new OrderPaid() //we look at the happy path 
            {
                AmountPaid = model.AmountPaid,
                OrderId = model.OrderId,
                PaymentMethod = model.PaymentMethod
            });
            return Ok("Payment Processor API");
        }

        [HttpGet("/requestcancelation/{id}")]
        public async Task<IActionResult> RequestCancelation(Guid id)
        {
            await messageSession.Publish(new CancelationRequested()
            {
                OrderId = id
            });

            return Accepted();
        }
    }
}
