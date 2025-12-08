using Contracts.Models;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contracts.Commands
{
    public class CreateOrder : ICommand
    {
        public Guid Id { get; set; }
        public OrderStatus Status { get; set; }

        public string DeliveryInstructions { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public List<OrderItemModel> OrderItems { get; set; }
    }
}
