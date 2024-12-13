using Microsoft.EntityFrameworkCore;
using CqrsDemo.Infrastructure.Persistence;
using CqrsDemo.Application;
using CqrsDemo.Application.Mapper;
using CqrsDemo.Infrastructure.Messaging;
using CqrsDemo.Application.Services;
using CqrsDemo.Infrastructure.Caching;
using CqrsDemo.Infrastructure.Repository; 
using CqrsDemo.Application.Services.OrderServices;
using CqrsDemo.Application.Models.DTOs.Order;
using CqrsDemo.Application.Handlers.Commands;
using CqrsDemo.Application.Commands.Order;
using MediatR;
using CqrsDemo.Application.Commands;
using CqrsDemo.Application.Handlers.Commands.Order;

var builder = WebApplication.CreateBuilder(args);

// Configure WriteDbContext for MSSQL (In-Memory for testing)
builder.Services.AddDbContext<WriteDbContext>(options =>
    options.UseInMemoryDatabase("WriteInMemoryOrder"));

// Configure ReadDbContext for PostgreSQL
builder.Services.AddDbContext<ReadDbContext>(options =>
 options.UseInMemoryDatabase("ReadInMemoryOrder"));
//options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresReadDb")));


// Register Redis cache as a singleton
builder.Services.AddSingleton<RedisCache>();

// Register RabbitMQConnectionManager as a singleton
builder.Services.AddSingleton<RabbitMQConnectionManager>();

// Register RabbitMQPublisher as a singleton
builder.Services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();

// Register RabbitMQConsumerService as a hosted service
builder.Services.AddHostedService(provider =>
    ActivatorUtilities.CreateInstance<RabbitMQConsumerService>(provider));



builder.Services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
builder.Services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));

builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));

builder.Services.AddScoped<OrderService>();

//builder.Services.Scan(scan => scan
//    .FromAssemblyOf<CreateOrderCommand>()
//    .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>))
//        .Where(type => !type.IsGenericTypeDefinition)) // Skip open generic types
//    .AsImplementedInterfaces()
//    .WithScopedLifetime()
//);
builder.Services.AddScoped<IRequestHandler<CreateOrderCommand, OrderDTO>, CreateOrderHandler>();
builder.Services.AddScoped<IRequestHandler<UpdateOrderCommand, OrderDTO>, UpdateOrderHandler>();
builder.Services.AddScoped<IRequestHandler<DeleteOrderCommand, Unit>, DeleteOrderHandler>();

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
