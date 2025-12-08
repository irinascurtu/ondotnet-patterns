using Contracts.Events;
using Contracts.Mappings;
using Contracts.Models;
using Microsoft.EntityFrameworkCore;
using NServiceBus.TransactionalSession;
using Ordering.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Service
{
    public class OrderIntakeService : IOrderIntakeService
    {
        private readonly IOrderRepository orderRepository;
        private readonly ITransactionalSession transactionalSession;

        public OrderIntakeService(IOrderRepository orderRepository, ITransactionalSession transactionalSession)
        {
            this.orderRepository = orderRepository;
            this.transactionalSession = transactionalSession;
        }

        public async Task AcceptOrder(OrderModel model)
        {
            //validate order
            await transactionalSession.Open(new SqlPersistenceOpenSessionOptions());

            var domainObject = model.ToOrder();

            var savedOrder = await orderRepository.AddOrderAsync(domainObject);

            var orderReceived = transactionalSession.Publish(new OrderReceived()
            {
                CreatedAt = savedOrder.OrderDate,
                OrderId = savedOrder.Id
            });

            var notifyOrderCreated = transactionalSession.Publish(new OrderCreated()
            {
                CreatedAt = savedOrder.OrderDate,
                OrderId = savedOrder.Id,
                TotalAmount = domainObject.OrderItems.Sum(x => x.Price * x.Quantity)
            });


            try
            {
                await orderRepository.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {
                // Handle exception appropriately
            }

            await transactionalSession.Commit();

        }
    }
}
