using Contracts.Commands;
using Contracts.Mutators;
using Microsoft.EntityFrameworkCore;
using NServiceBus.MessageMutator;
using Ordering.Data;
using Ordering.Domain;
using Ordering.Service;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OrderContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());

});

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();


var endpointConfiguration = new NServiceBus.EndpointConfiguration("Ordering.Api");

endpointConfiguration.UseSerialization<SystemJsonSerializer>();
endpointConfiguration.EnableCallbacks();
endpointConfiguration.MakeInstanceUniquelyAddressable("Instance1");

var recoverability = endpointConfiguration.Recoverability();
recoverability.AddUnrecoverableException<ValidationException>();
recoverability.AddUnrecoverableException(typeof(ArgumentException));
recoverability.Immediate(r =>
{
    r.NumberOfRetries(1);
});

recoverability.Delayed(delayed =>
{
    delayed.NumberOfRetries(5);
    delayed.TimeIncrease(TimeSpan.FromSeconds(2));//sets the delay for each
});



var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
transport.UseConventionalRoutingTopology(QueueType.Quorum);
transport.ConnectionString("host=localhost;username=guest;password=guest");

transport.Routing().RouteToEndpoint(typeof(CreateOrder), "OrdersProcessor");
transport.Routing().RouteToEndpoint(typeof(VerifyOrder), "OrdersProcessor");
endpointConfiguration.AddHeaderToAllOutgoingMessages("MyGlobalHeader", "My cool generic value");
endpointConfiguration.RegisterMessageMutator(new ProcessingPriorityMutator());
builder.Services.AddHttpContextAccessor();
//builder.Services.AddSingleton<IMutateOutgoingTransportMessages>(provider =>
// new AddTenantFromHttpContextMutator(provider.GetRequiredService<IHttpContextAccessor>()));

//var conventions = endpointConfiguration.Conventions();

//conventions.DefiningTimeToBeReceivedAs(
//     type =>
//     {
//         if (type == typeof(CreateOrder))
//         {
//             return TimeSpan.FromMinutes(1);
//         }
//         return TimeSpan.MaxValue;
//     });


endpointConfiguration.EnableInstallers();

builder.UseNServiceBus(endpointConfiguration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            serviceScope.ServiceProvider.GetService<OrderContext>().Database.EnsureCreated();
        }
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
