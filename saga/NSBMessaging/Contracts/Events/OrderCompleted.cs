using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Events
{
    public class OrderCompleted : IEvent
    {
        public Guid OrderId { get; set; }
    }

    public class OrderRefunded : IEvent
    {
        public Guid OrderId { get; set; }
    }
}
