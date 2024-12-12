using Microsoft.EntityFrameworkCore;
using CqrsDemo.Infrastructure.Persistence;
using CqrsDemo.Application; 
using CqrsDemo.Application.Mapper;
using CqrsDemo.Infrastructure.Messaging;
using CqrsDemo.Application.Services;
using CqrsDemo.Infrastructure.Caching;
using CqrsDemo.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WriteDbContext>(options =>
    options.UseInMemoryDatabase("OrderDb"));

// Register ReadDbContext that uses postgresql
builder.Services.AddDbContext<ReadDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ReadDbContext")));

// Register Redis cache as a singleton
builder.Services.AddSingleton<RedisCache>();

// Register RabbitMQConnectionManager as a singleton
builder.Services.AddSingleton<RabbitMQConnectionManager>();

// Register RabbitMQPublisher as a singleton
builder.Services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();

// Register RabbitMQConsumerService as a hosted service
builder.Services.AddHostedService<RabbitMQConsumerService>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(WriteRepository<>));
builder.Services.AddScoped(typeof(IRepository<>), typeof(ReadRepository<>));


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
