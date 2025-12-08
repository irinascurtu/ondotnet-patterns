using Contracts.Commands;
using Contracts.Messages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrdersProcessor
{
    public class VerifyOrderHandler(ILogger<VerifyOrderHandler> log) : IHandleMessages<VerifyOrder>
    {
        public async Task Handle(VerifyOrder message, IMessageHandlerContext context)
        {
            //call the database
            var response = new OrderResult()
            {
                IsValid = true
            };
            await context.Reply(response);
        }
    }
}
