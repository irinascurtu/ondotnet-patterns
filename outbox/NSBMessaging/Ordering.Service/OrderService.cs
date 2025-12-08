using AutoMapper;
using Contracts.Events;
using Contracts.Mappings;
using Contracts.Models;
using Microsoft.EntityFrameworkCore;
using Ordering.Data;
using Ordering.Domain.Entities;

namespace Ordering.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _orderRepository.GetOrdersAsync();
        }

        public async Task<Order> GetOrderAsync(Guid id)
        {
            return await _orderRepository.GetOrderAsync(id);
        }


        public async Task<Order> AddOrderAsync(Order order)
        {
            return await _orderRepository.AddOrderAsync(order);

        }

        public async Task<int> UpdateOrderAsync(Order order)
        {
            return await _orderRepository.UpdateOrderAsync(order);
        }

        public async Task DeleteOrderAsync(Guid id)
        {
            await _orderRepository.DeleteOrderAsync(id);
        }

        public async Task<bool> OrderExistsAsync(Guid id)
        {
            return await _orderRepository.OrderExistsAsync(id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _orderRepository.SaveChangesAsync();
        }

        public async Task AcceptOrder(OrderModel model)
        {
            //validateOrder

            var domainObject = model.ToOrder();


            var savedOrder = await this.AddOrderAsync(domainObject);

            //var orderReceived = publish.Publish(new OrderReceived()
            //{
            //    CreatedAt = savedOrder.OrderDate,
            //    OrderId = savedOrder.Id
            //});


            //var notifyOrderCreated = .Publish(new OrderCreated()
            //{
            //    CreatedAt = savedOrder.OrderDate,
            //    OrderId = savedOrder.Id,
            //    TotalAmount = domainObject.OrderItems.Sum(x => x.Price * x.Quantity)
            //});

            try
            {
                await _orderRepository.SaveChangesAsync();
            }
            catch (DbUpdateException exception)
            {

            }

        }

    }
}
