using Microsoft.Extensions.DependencyInjection;

namespace mediateur;

public static class MediateurRegistrer
{
    
    
    public static void RegisterNotificationHandlers<I>(this IServiceCollection services) where I : INotificationHandler

    {
        var type = typeof(I);
        
        //RegisterHandlersFromAssemblyContaining<I>(services);
        if (!type.IsInterface)
            throw new ArgumentException("T doit être une interface");
        var assemblies = AppDomain.CurrentDomain
            .GetAssemblies();
        foreach (var assembly in assemblies)
        {
            Console.WriteLine(assembly.GetName().Name);
        }
        
        var handlerTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a =>
            {
                var types = a.GetTypes()

                    .Where(t => t is { IsClass: true, IsAbstract: false }

                                && t.IsAssignableTo(typeof(INotificationHandler)))

                    .ToList();
                return types;
            }).ToList();


        List<I> instances = new List<I>();
        
        foreach (var implementationType in handlerTypes)

        {

            var interfaceType = implementationType.GetInterfaces()

                .FirstOrDefault(i => i != typeof(INotificationHandler) && i.IsAssignableTo(typeof(INotificationHandler)));

    

            if (interfaceType is not null)

            {
                var instance= Activator.CreateInstance(implementationType);
                instances.Add((I)instance);
            }

        }

        var aggregator = (I)DynamicTypeBuilder.CreateTypeForNotificationHandler<I>(instances);
        services.AddSingleton(type,aggregator);
        

    }

    public static void RegisterRequestHandlers<I>(this IServiceCollection services) where I : IRequestHandler
    {
        var type = typeof(I);
        var assembly = typeof(I).Assembly;
        if (!type.IsInterface)
            throw new ArgumentException($"type {typeof(I).Name} must be an interface");

        
        var handlerTypes = assembly.GetTypes()

            .Where(t => t is { IsClass: true, IsAbstract: false }

                        && t.IsAssignableTo(typeof(IRequestHandler)))

            .ToList();

        if (handlerTypes.Count > 1)
        {
            throw new ArgumentException($"Only one implementation of {typeof(I).Name} can exist.");
        }

        if (handlerTypes.Count == 1)
        {
            services.AddTransient(typeof(I), handlerTypes.First());
        }

        // List<I> instances = new List<I>();
        //
        // foreach (var implementationType in handlerTypes)
        //
        // {
        //
        //     var interfaceType = implementationType.GetInterfaces()
        //
        //         .FirstOrDefault(i => i != typeof(IRequestHandler) && i.IsAssignableTo(typeof(IRequestHandler)));
        //
        //
        //
        //     if (interfaceType is not null)
        //
        //     {
        //         var instance= Activator.CreateInstance(implementationType);
        //         instances.Add((I)instance);
        //     }
        //
        // }
        //
        // //var instances = services.OfType<I>().ToList();
        //
        // var aggregator = DynamicTypeBuilder.CreateTypeForNotificationHandler<I>(instances);
        // services.AddSingleton(type,aggregator);
    }
}