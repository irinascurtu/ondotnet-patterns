using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Events
{
    public class OrderPaid : IEvent
    {
        public Guid OrderId { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentMethod { get; set; }

    }
}
