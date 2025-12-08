using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ordering.Domain;

var builder = Host.CreateApplicationBuilder(args);

// TODO: consider moving common endpoint configuration into a shared project
// for use by all endpoints in the system

builder.Services.AddDbContext<OrderContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());

});

var endpointConfiguration = new EndpointConfiguration("Ordering.Saga");

var transport = endpointConfiguration.UseTransport<RabbitMQTransport>();
transport.UseConventionalRoutingTopology(QueueType.Quorum);
transport.ConnectionString("host=localhost;username=guest;password=guest");

endpointConfiguration.UseSerialization<SystemJsonSerializer>();
endpointConfiguration.AuditProcessedMessagesTo("audit");
endpointConfiguration.AuditSagaStateChanges(serviceControlQueue: "audit");
//persistence
var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
persistence.ConnectionBuilder(() => new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
persistence.SqlDialect<SqlDialect.MsSqlServer>();

endpointConfiguration.EnableInstallers();

builder.UseNServiceBus(endpointConfiguration);

var app = builder.Build();
app.Run();
