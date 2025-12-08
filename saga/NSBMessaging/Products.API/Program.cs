using Microsoft.EntityFrameworkCore;
using Products.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{

    options.ReturnHttpNotAcceptable = true;
    options.AllowEmptyInputInBodyModelBinding = true;
    options.RespectBrowserAcceptHeader = true;
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();






builder.Services.AddDbContext<ProductContext>(options =>
options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"))
        .LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                .EnableSensitiveDataLogging()); // Optional: shows parameter values in SQL logs);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
     using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<ProductContext>().Database.EnsureCreated();
                    serviceScope.ServiceProvider.GetService<ProductContext>().EnsureSeeded();
                }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
