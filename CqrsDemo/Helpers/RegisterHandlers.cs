using Autofac;
using CqrsDemo.Application.Commands;
using CqrsDemo.Application.Commands.Order;
using CqrsDemo.Application.Handlers.Commands;
using CqrsDemo.Application.Handlers.Commands.Order;
using CqrsDemo.Application.Models.DTOs.Order;
using MediatR;
using System.Reflection;

namespace CqrsDemo.Api.Helpers
{
    public static class RegisterHandlers
    {
        public static void RegisterCommandHandlers(this ContainerBuilder containerBuilder, Assembly assembly)
        {
            var handlerBaseTypes = new[]
            {
        typeof(CreateCommandHandler<,>),
        typeof(UpdateCommandHandler<,>),
        typeof(DeleteCommandHandler<,>)
    };

            var handlerTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract) // Only concrete classes
                .Where(t => GetGenericBaseType(t) != null) // Only types with one of the desired base types
                .ToList();

            foreach (var handlerType in handlerTypes)
            {
                try
                {
                    var baseType = GetGenericBaseType(handlerType);
                    if (baseType == null) continue;

                    var genericArguments = baseType.GetGenericArguments();
                    if (genericArguments.Length == 2)
                    {
                        var entityType = genericArguments[0];
                        var dtoType = genericArguments[1];

                        // 1. Get the *actual* command type, not just the base type
                        Type commandType = null;
                        if (baseType.GetGenericTypeDefinition() == typeof(CreateCommandHandler<,>))
                        {
                            commandType = assembly.GetTypes()
                                .FirstOrDefault(t =>
                                    t.IsClass &&
                                    !t.IsAbstract &&
                                    t.BaseType != null &&
                                    t.BaseType.IsGenericType &&
                                    t.BaseType.GetGenericTypeDefinition() == typeof(CreateCommand<,>) &&
                                    t.BaseType.GetGenericArguments()[0] == entityType &&
                                    t.BaseType.GetGenericArguments()[1] == dtoType
                                );
                        }
                        else if (baseType.GetGenericTypeDefinition() == typeof(UpdateCommandHandler<,>))
                        {
                            commandType = assembly.GetTypes()
                                .FirstOrDefault(t =>
                                    t.IsClass &&
                                    !t.IsAbstract &&
                                    t.BaseType != null &&
                                    t.BaseType.IsGenericType &&
                                    t.BaseType.GetGenericTypeDefinition() == typeof(UpdateCommand<,>) &&
                                    t.BaseType.GetGenericArguments()[0] == entityType &&
                                    t.BaseType.GetGenericArguments()[1] == dtoType
                                );
                        }
                        else if (baseType.GetGenericTypeDefinition() == typeof(DeleteCommandHandler<,>))
                        {
                            commandType = assembly.GetTypes()
                                .FirstOrDefault(t =>
                                    t.IsClass &&
                                    !t.IsAbstract &&
                                    t.BaseType != null &&
                                    t.BaseType.IsGenericType &&
                                    t.BaseType.GetGenericTypeDefinition() == typeof(DeleteCommand<>) &&
                                    t.BaseType.GetGenericArguments()[0] == entityType
                                );
                            dtoType = typeof(Unit); // Delete handlers typically return Unit
                        }

                        if (commandType != null)
                        {
                            var requestHandlerInterface = typeof(IRequestHandler<,>).MakeGenericType(commandType, dtoType);
                            containerBuilder.RegisterType(handlerType)
                                .As(requestHandlerInterface)
                                .InstancePerLifetimeScope();

                            Console.WriteLine($"Registered {handlerType.Name} for {commandType.Name} and {dtoType.Name}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error registering handler {handlerType.Name}: {ex.Message}");
                }
            }
        }

        // Helper method to get the base type
        public static Type GetGenericBaseType(Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && (
                    type.GetGenericTypeDefinition() == typeof(CreateCommandHandler<,>) ||
                    type.GetGenericTypeDefinition() == typeof(UpdateCommandHandler<,>) ||
                    type.GetGenericTypeDefinition() == typeof(DeleteCommandHandler<,>)))
                {
                    return type;
                }
                type = type.BaseType;
            }
            return null;
        }
    }
}
