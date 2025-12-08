using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// TODO: consider moving common endpoint configuration into a shared project
// for use by all endpoints in the system

var endpointConfiguration = new EndpointConfiguration("AdminNotification.Worker");

// RabbitMQ Transport: https://docs.particular.net/transports/rabbitmq/
var rabbitMqConnectionString = "CONNECTION_STRING";
var transport = new RabbitMQTransport(RoutingTopology.Conventional(QueueType.Quorum), rabbitMqConnectionString);
var routing = endpointConfiguration.UseTransport(transport);

// Define routing for commands: https://docs.particular.net/nservicebus/messaging/routing#command-routing
// routing.RouteToEndpoint(typeof(MessageType), "DestinationEndpointForType");
// routing.RouteToEndpoint(typeof(MessageType).Assembly, "DestinationForAllCommandsInAssembly");

// SQL Persistence: https://docs.particular.net/persistence/sql/
// Microsoft SQL Server dialect: https://docs.particular.net/persistence/sql/dialect-mssql
var dbConnectionString = "Data Source=.\\SqlExpress;Initial Catalog=dbname;Integrated Security=True;";
var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
persistence.SqlDialect<SqlDialect.MsSqlServer>();
persistence.ConnectionBuilder(() => new SqlConnection(dbConnectionString));

// Message serialization
endpointConfiguration.UseSerialization<SystemJsonSerializer>();

endpointConfiguration.DefineCriticalErrorAction(OnCriticalError);

// Installers are useful in development. Consider disabling in production.
// https://docs.particular.net/nservicebus/operations/installers
endpointConfiguration.EnableInstallers();

builder.UseNServiceBus(endpointConfiguration);

var app = builder.Build();
app.Run();

static async Task OnCriticalError(ICriticalErrorContext context, CancellationToken cancellationToken)
{
    // TODO: decide if stopping the endpoint and exiting the process is the best response to a critical error
    // https://docs.particular.net/nservicebus/hosting/critical-errors
    // and consider setting up service recovery
    // https://docs.particular.net/nservicebus/hosting/windows-service#installation-restart-recovery
    try
    {
        await context.Stop(cancellationToken);
    }
    finally
    {
        FailFast($"Critical error, shutting down: {context.Error}", context.Exception);
    }
}

static void FailFast(string message, Exception exception)
{
    try
    {
        // TODO: decide what kind of last resort logging is necessary
        // TODO: when using an external logging framework it is important to flush any pending entries prior to calling FailFast
        // https://docs.particular.net/nservicebus/hosting/critical-errors#when-to-override-the-default-critical-error-action
    }
    finally
    {
        Environment.FailFast(message, exception);
    }
}
