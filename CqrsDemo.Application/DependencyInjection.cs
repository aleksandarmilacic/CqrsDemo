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

       
    }
}
