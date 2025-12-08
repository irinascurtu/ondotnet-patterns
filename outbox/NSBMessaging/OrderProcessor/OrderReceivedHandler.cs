using Contracts.Events;
using Contracts.Exceptions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace OrdersProcessor;

internal class OrderReceivedHandler(ILogger<OrderReceivedHandler> log) : IHandleMessages<OrderReceived>
{
    public async Task Handle(OrderReceived message, IMessageHandlerContext context)
    {
        log.LogInformation("I log from OrderReceivedHandler");
        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        Console.WriteLine(json);
        // throw new OrderReceivedException();

        await Task.CompletedTask;
    }
}
