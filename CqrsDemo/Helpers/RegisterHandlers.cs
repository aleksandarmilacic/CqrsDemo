using Autofac;
using CqrsDemo.Application.Commands;
using CqrsDemo.Application.Handlers.Commands;
using MediatR;
using System.Reflection;

namespace CqrsDemo.Api.Helpers
{
    public static class RegisterHandlers
    {
        // Single source of truth for Handler <-> Command mapping
        private static readonly Dictionary<Type, Type> HandlerCommandMappings = new()
        {
            { typeof(CreateCommandHandler<,>), typeof(CreateCommand<,>) },
            { typeof(UpdateCommandHandler<,>), typeof(UpdateCommand<,>) },
            { typeof(DeleteCommandHandler<,>), typeof(DeleteCommand<>) }
        };

        public static void RegisterCommandHandlers(this ContainerBuilder containerBuilder, Assembly assembly)
        {
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
                    if (genericArguments.Length != 2) continue;

                    var entityType = genericArguments[0];
                    var dtoType = genericArguments[1];

                    var commandType = GetDerivedCommandType(assembly, baseType, entityType, dtoType);

                    if (baseType.GetGenericTypeDefinition() == typeof(DeleteCommandHandler<,>))
                    {
                        dtoType = typeof(Unit);
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Error registering handler {handlerType.Name}: {ex.Message}");
                }
            }
        }

        // Dynamically resolve the command type using the handler's base type
        private static Type GetDerivedCommandType(Assembly assembly, Type handlerBaseType, Type entityType, Type dtoType)
        {
            if (!HandlerCommandMappings.TryGetValue(handlerBaseType.GetGenericTypeDefinition(), out var commandBaseType))
                return null;

            // Special handling for DeleteCommand which only has one generic argument
            if (commandBaseType == typeof(DeleteCommand<>))
            {
                return assembly.GetTypes()
                    .FirstOrDefault(t =>
                        t.IsClass &&
                        !t.IsAbstract &&
                        t.BaseType != null &&
                        t.BaseType.IsGenericType &&
                        t.BaseType.GetGenericTypeDefinition() == commandBaseType &&
                        t.BaseType.GetGenericArguments().SequenceEqual(new[] { entityType })
                    );
            }

            // Handling for CreateCommand and UpdateCommand which have two generic arguments
            return assembly.GetTypes()
                .FirstOrDefault(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    t.BaseType != null &&
                    t.BaseType.IsGenericType &&
                    t.BaseType.GetGenericTypeDefinition() == commandBaseType &&
                    t.BaseType.GetGenericArguments().SequenceEqual(new[] { entityType, dtoType })
                );
        }

        private static Type GetGenericBaseType(Type type)
        {
            while (type != null && type != typeof(object))
            {
                if (type.IsGenericType && HandlerCommandMappings.ContainsKey(type.GetGenericTypeDefinition()))
                {
                    return type;
                }
                type = type.BaseType;
            }
            return null;
        }
    }
}
