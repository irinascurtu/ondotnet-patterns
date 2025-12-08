using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Events
{
    public class OrderReceived : IEvent
    {
        public Guid OrderId { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public OrderStatus Status { get; init; }
        public decimal TotalAmount { get; init; }

        public string? Notes { get; init; }
        public Guid CustomerId { get; init; }
    }
}
