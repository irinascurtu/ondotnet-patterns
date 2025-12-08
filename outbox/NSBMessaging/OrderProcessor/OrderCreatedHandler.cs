using Contracts.Events;
using Contracts.Exceptions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace OrdersProcessor;

internal class OrderCreatedHandler(ILogger<OrderCreatedHandler> log) : IHandleMessages<OrderCreated>
{
    public async Task Handle(OrderCreated message, IMessageHandlerContext context)
    {
        log.LogInformation("I log from OrderCreatedHandler");
        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        Console.WriteLine(json);

      //  throw new OrderCreatedException();


        await Task.CompletedTask;
    }
}
