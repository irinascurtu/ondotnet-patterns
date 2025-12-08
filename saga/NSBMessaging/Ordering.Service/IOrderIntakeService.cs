using Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Service
{
    public interface IOrderIntakeService
    {
        Task AcceptOrder(OrderModel model);
    }
}
