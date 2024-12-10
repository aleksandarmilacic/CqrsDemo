using Microsoft.EntityFrameworkCore;
using CqrsDemo.Infrastructure.Persistence;
using CqrsDemo.Application; 
using CqrsDemo.Application.Mapper;
using CqrsDemo.Infrastructure.Messaging;
using CqrsDemo.Application.Services;
using CqrsDemo.Infrastructure.Caching;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("OrderDb"));

// Add the Redis cache
builder.Services.AddSingleton<RedisCache>();

// Add the RabbitMQ 
builder.Services.AddSingleton<RabbitMQConnectionManager>();

// Add the RabbitMQ consumer as a hosted service
builder.Services.AddHostedService<RabbitMQConsumerService>();

// Add application-level messaging service
builder.Services.AddScoped<IRabbitMQPublisher, RabbitMQPublisher>();


builder.Services.AddApplication();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register Automapper
builder.Services.AddAutoMapper(typeof(OrderMapping));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();
app.Run();
