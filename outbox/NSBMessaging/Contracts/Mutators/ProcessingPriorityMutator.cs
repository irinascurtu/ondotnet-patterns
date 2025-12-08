using Contracts.Commands;
using NServiceBus.MessageMutator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Mutators
{
    public class ProcessingPriorityMutator : IMutateOutgoingMessages
    {
        public Task MutateOutgoing(MutateOutgoingMessageContext context)
        {
            var priority = "Normal";

            if (context.OutgoingMessage is CreateOrder order)
            {
                if (order.OrderItems.Count >= 3)
                    priority = "High";
            }

            context.OutgoingHeaders["ProcessingPriority"] = priority;
            return Task.CompletedTask;
        }
    }
}
