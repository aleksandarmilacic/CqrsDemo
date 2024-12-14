using Autofac;
using CqrsDemo.Application.Commands;
using CqrsDemo.Application.Commands.Order;
using CqrsDemo.Application.Handlers.Commands;
using CqrsDemo.Application.Handlers.Commands.Order;
using CqrsDemo.Application.Models.DTOs;
using CqrsDemo.Application.Models.DTOs.Order;
using CqrsDemo.Application.Services;
using CqrsDemo.Application.Services.OrderServices;
using CqrsDemo.Domain.Entities.Order;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace CqrsDemo.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            
            services.AddMediatR(cf => cf.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            return services;

        }

        public static ContainerBuilder AddMediatrHandlersWithOpenGeneric(this ContainerBuilder builder)
        {
            //  builder.RegisterType<CreateOrderHandler>()
            //.As<IRequestHandler<CreateOrderCommand, OrderDTO>>()
            //.InstancePerLifetimeScope();

            //  builder.RegisterType<UpdateOrderHandler>()
            //      .As<IRequestHandler<UpdateOrderCommand, OrderDTO>>()
            //      .InstancePerLifetimeScope();

            //  builder.RegisterType<DeleteOrderHandler>()
            //      .As<IRequestHandler<DeleteOrderCommand, Unit>>()
            //      .InstancePerLifetimeScope();
            // Register CreateOrderHandler's Assembly for IRequestHandler
            builder.RegisterAssemblyTypes(typeof(CreateOrderHandler).Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>))
                .InstancePerLifetimeScope();

            // Register Generic Handlers
            builder.RegisterGeneric(typeof(CreateCommandHandler<,>))
                .As(typeof(IRequestHandler<CreateCommand<object, IDTO>, IDTO>))
                .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(UpdateCommandHandler<,>))
                .As(typeof(IRequestHandler<UpdateCommand<object, IDTO>, IDTO>))
                .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(DeleteCommandHandler<,>))
                .As(typeof(IRequestHandler<DeleteCommand<object>, Unit>))
                .InstancePerLifetimeScope();
 
            return builder;
        }
    }
}
