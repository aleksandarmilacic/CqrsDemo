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
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CqrsDemo.Application.Models.DTOs;
using CqrsDemo.Api.Helpers;
using System.Reflection;
using CqrsDemo.Domain.Entities;



var builder = WebApplication.CreateBuilder(args);


builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

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

//builder.Services.AddScoped<OrderService>();
//builder.Services.Scan(scan => scan
//    .FromAssemblyOf<GenericService<object, CqrsDemo.Application.Models.DTOs.IDTO>>() // Scan the assembly where GenericService is located
//    .AddClasses(classes => classes.AssignableTo(typeof(GenericService<,>))) // Find all classes that inherit GenericService<T, TDTO>
//    .AsSelf() // Register as self (so you can inject 'OrderService' directly)
//    .WithScopedLifetime() // Scoped lifetime for services
//);

builder.Services.AddApplication();
  

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{

    // Usage in the containerBuilder
    containerBuilder.RegisterAssemblyTypes(typeof(GenericService<IEntity<Guid>, CqrsDemo.Application.Models.DTOs.IDTO>).Assembly) // Scan the assembly where GenericService is located
        .Where(t => t.IsAssignableToGenericType(typeof(GenericService<,>))) // Find all classes that inherit GenericService<T, TDTO>
        .AsSelf() // Register as self (so you can inject 'OrderService' directly)
        .InstancePerLifetimeScope(); // Scoped lifetime for services 
                                     // Register all IRequestHandler implementations generically

    // Explicit registration for IOrderService and OrderService
    containerBuilder.RegisterType<OrderService>()
        .As<IOrderService>()
        .InstancePerLifetimeScope();

    //containerBuilder.AddMediatrHandlersWithOpenGeneric();
    containerBuilder.RegisterGeneric(typeof(WriteRepository<>)).As(typeof(IWriteRepository<>)).InstancePerLifetimeScope();
    containerBuilder.RegisterGeneric(typeof(ReadRepository<>)).As(typeof(IReadRepository<>)).InstancePerLifetimeScope();

    containerBuilder.RegisterAssemblyTypes(typeof(CreateOrderHandler).Assembly)
            .AsClosedTypesOf(typeof(IRequestHandler<,>))
            .InstancePerLifetimeScope();

    //containerBuilder.RegisterType<CreateOrderHandler>()
    //       .As<IRequestHandler<CreateOrderCommand, OrderDTO>>()
    //       .InstancePerLifetimeScope();

    //containerBuilder.RegisterType<UpdateOrderHandler>()
    //       .As<IRequestHandler<UpdateOrderCommand, OrderDTO>>()
    //       .InstancePerLifetimeScope();

    //containerBuilder.RegisterType<DeleteOrderHandler>()
    //       .As<IRequestHandler<DeleteOrderCommand, Unit>>()
    //       .InstancePerLifetimeScope();



});

//builder.Services.AddScoped<IRequestHandler<CreateOrderCommand, OrderDTO>, CreateOrderHandler>();
//builder.Services.AddScoped<IRequestHandler<UpdateOrderCommand, OrderDTO>, UpdateOrderHandler>();
//builder.Services.AddScoped<IRequestHandler<DeleteOrderCommand, Unit>, DeleteOrderHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Automapper
builder.Services.AddAutoMapper(typeof(OrderMapping));

// Adds logging to the dependency injection container
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();


app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    //app.UseDeveloperExceptionPage();
    
}

app.UseRouting();
app.MapControllers();
app.Run();
