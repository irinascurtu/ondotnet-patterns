using Contracts.Behaviors;
using Contracts.Events;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus.Logging;
using Ordering.Data;
using Ordering.Domain;
using Ordering.Service;
using System.ComponentModel.DataAnnotations;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddDbContext<OrderContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());

});

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

var endpointConfiguration = new EndpointConfiguration("OrdersProcessor");

var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
transport.UseConventionalRoutingTopology(QueueType.Quorum);
transport.ConnectionString("host=localhost;username=guest;password=guest");
//persistence
var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
persistence.ConnectionBuilder(() => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
persistence.SqlDialect<SqlDialect.MsSqlServer>();
var outboxSettings = endpointConfiguration.EnableOutbox();
outboxSettings.UsePessimisticConcurrencyControl();


var recoverability = endpointConfiguration.Recoverability();
//recoverability.AddUnrecoverableException<ArgumentException>();
//recoverability.AddUnrecoverableException(typeof(ArgumentException));
recoverability.Immediate(r =>
{
    r.NumberOfRetries(0);
    r.OnMessageBeingRetried((retry, ct) =>
    {
        Console.WriteLine("I'm a retried message");
        //log.Info($"Message {retry.MessageId} will be retried immediately.");
        return Task.CompletedTask;
    });
});


recoverability.Delayed(delayed =>
{
    delayed.NumberOfRetries(0);
    delayed.TimeIncrease(TimeSpan.FromSeconds(2));//sets the delay for each
    delayed.OnMessageBeingRetried((retry, ct) =>
    {
        // log.Info($@"Message {retry.MessageId} will be retried after a delay.");
        Console.WriteLine($"Wuhuhu!!! Delayed, retried");

        return Task.CompletedTask;
    });
});

recoverability.Failed(settings => settings.OnMessageSentToErrorQueue((failed, ct) =>
{
    // log.Fatal($@"Message {failed.MessageId} will be sent to the error queue.");
    Console.WriteLine($"Wuhuhu!!! Road to error queue");
    return Task.CompletedTask;
}));


endpointConfiguration.Recoverability().OnConsecutiveFailures(2,
    new RateLimitSettings(
        timeToWaitBetweenThrottledAttempts: TimeSpan.FromMinutes(1),
        onRateLimitStarted: ct => Console.Out.WriteLineAsync("Rate limiting started"),
        onRateLimitEnded: cts => Console.Out.WriteLineAsync("Rate limiting stopped")));


endpointConfiguration.UseSerialization<SystemJsonSerializer>();

//endpointConfiguration.Pipeline.Register(new EnsureTenantHeaderBehavior(), 
//    "Ensures the TenantId header is there");

endpointConfiguration.Pipeline.Register(new PropagateTenantHeaderBehavior(),
    "Adds the tenant");

endpointConfiguration.EnableInstallers();

builder.UseNServiceBus(endpointConfiguration);

var app = builder.Build();
app.Run();