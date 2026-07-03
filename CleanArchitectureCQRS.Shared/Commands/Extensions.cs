using System.Reflection;
using CleanArchitectureCQRS.Shared.Abstractions.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureCQRS.Shared.Commands;

    public static class Extensions
    {
        public static IServiceCollection AddCommands(this IServiceCollection services, params Assembly[] assemblies)
        {
            var scanAssemblies = assemblies is { Length: > 0 }
                ? assemblies
                : new[] { Assembly.GetCallingAssembly() };

            services.AddSingleton<ICommandDispatcher, InMemoryCommandDispatcher>();
            services.Scan(s => s.FromAssemblies(scanAssemblies)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());
            return services;
        }
    }

