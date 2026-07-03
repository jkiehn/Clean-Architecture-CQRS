using System.Reflection;
using CleanArchitectureCQRS.Shared.Abstractions.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureCQRS.Shared.Queries;

    public static class Extensions
    {
        public static IServiceCollection AddQueries(this IServiceCollection services, params Assembly[] assemblies)
        {
            var scanAssemblies = assemblies is { Length: > 0 }
                ? assemblies
                : new[] { Assembly.GetCallingAssembly() };

            services.AddSingleton<IQueryDispatcher, InMemoryQueryDispatcher>();
            services.Scan(s => s.FromAssemblies(scanAssemblies)
                .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }

