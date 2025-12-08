using Contracts.Commands;
using Contracts.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var endpointConfiguration = new NServiceBus.EndpointConfiguration("PaymentsProcessor");

endpointConfiguration.UseSerialization<SystemJsonSerializer>();

var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
transport.UseConventionalRoutingTopology(QueueType.Quorum);
transport.ConnectionString("host=localhost;username=guest;password=guest");


transport.Routing().RouteToEndpoint(typeof(CancelationRequested), "Ordering.Saga");
transport.Routing().RouteToEndpoint(typeof(OrderPaid), "Ordering.Saga");

endpointConfiguration.EnableInstallers();

builder.UseNServiceBus(endpointConfiguration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
