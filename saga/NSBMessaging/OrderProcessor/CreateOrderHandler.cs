using Contracts.Commands;
using Contracts.Events;
using Contracts.Mappings;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;
using Ordering.Service;
using System.Text.Json;

namespace OrderProcessor;

public class CreateOrderHandler(ILogger<CreateOrderHandler> log, IOrderService orderService) : IHandleMessages<CreateOrder>
{
    public async Task Handle(CreateOrder message, IMessageHandlerContext context)
    {

        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            WriteIndented = true
        });


        Console.WriteLine(json);

        var order = message.ToOrder();

        await context.Publish(new OrderReceived
        {
            OrderId = message.Id,
            CreatedAt = DateTime.UtcNow
        });

        //save in database
        var savedOrder = await orderService.AddOrderAsync(order);
        await context.Publish(new OrderCreated
        {
            CreatedAt = DateTime.UtcNow,
            CustomerId = order.CustomerId,
            Currency = "USD",
            OrderId = message.Id,
            TotalAmount = order.Total
        });

        await orderService.SaveChangesAsync();

        await Task.CompletedTask;

    }
}
