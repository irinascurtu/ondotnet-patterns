using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Saga
{
    public class OrderSagaData : ContainSagaData
    {
        public Guid OrderId { get; set; }//correlation property
        public decimal Amount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? CanceledAt { get; set; }

        public bool IsBilled { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public string CurrentState { get; set; }//not required by NSB 

    }
}
