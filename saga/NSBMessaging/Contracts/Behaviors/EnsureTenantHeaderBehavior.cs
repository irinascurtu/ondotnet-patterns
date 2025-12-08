using NServiceBus.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Behaviors
{
    public class EnsureTenantHeaderBehavior : Behavior<IIncomingLogicalMessageContext>
    {
        public override Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            if (!context.Headers.ContainsKey("TenantId"))
            {
                throw new InvalidOperationException("Missing TenantId header");
            }
            return next();
        }
    }
}
