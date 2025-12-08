using Contracts.Commands;
using Contracts.Models;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Mappings
{
    public static class CreateOrderRequestExtensions
    {
        public static Customer ToCustomer(this OrderModel model)
        {
            return new Customer
            {
                Id = Guid.NewGuid(),
                Name = model.CustomerName,
                Phone = model.Phone,
                Email = model.Email
            };
        }

        public static Customer ToCustomer(this CreateOrder orderCommand)
        {
            return new Customer
            {
                Id = Guid.NewGuid(),
                Name = orderCommand.CustomerName,
                Phone = orderCommand.Phone,
                Email = orderCommand.Email
            };
        }

        public static Order ToOrder(this OrderModel model)
        {
            return new Order
            {
                Id = Guid.NewGuid(),
                Customer = model.ToCustomer(),
                DeliveryInstructions = model.DeliveryInstructions,
                OrderItems = model.OrderItems.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList(),
                Total = model.OrderItems.Sum(x => x.Quantity * x.Price),
            };
        }

        public static Order ToOrder(this CreateOrder orderCommand)
        {
            return new Order
            {
                Id = orderCommand.Id,
                Customer = orderCommand.ToCustomer(),
                OrderDate=DateTime.UtcNow,
                DeliveryInstructions = orderCommand.DeliveryInstructions,
                OrderItems = orderCommand.OrderItems.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList(),
                Total = orderCommand.OrderItems.Sum(x => x.Quantity * x.Price),
            };
        }
    }

}