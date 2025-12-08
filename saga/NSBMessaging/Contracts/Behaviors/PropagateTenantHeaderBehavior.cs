using Contracts.Events;
using NServiceBus.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Behaviors
{
    public class PropagateTenantHeaderBehavior : Behavior<IOutgoingLogicalMessageContext>
    {
        public override Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
        {

            if (context.TryGetIncomingPhysicalMessage(out var incomingMessage))
            {
                if (incomingMessage.Headers.TryGetValue("TenantId", out var incomingTenantId))
                {
                    context.Headers["TenantId"] = incomingTenantId;
                }
            }

            if (context.Message.Instance is OrderReceived or OrderCreated)
            {
                
            }

            return next();
        }
    }
}
