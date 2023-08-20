namespace MarketData.API.Extensions;

using MarketData.Application.Interfaces;
using MarketData.Application.Repositories;
using MarketData.Infrastructure.Repositories;
using MarketData.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public static class ServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
    }

    public static void AddApplicationServices(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (Assembly assembly in assemblies)
        {
            var allTypes = assembly
                .GetTypes()
                .ToList();
            foreach (var type in allTypes)
            {
                var interfaceType = type.GetInterfaces().FirstOrDefault(e => e.Name == "I" + type.Name);
                if (interfaceType != null)
                {
                    if (typeof(ISingletonService).IsAssignableFrom(type))
                    {
                        services.AddSingleton(interfaceType, type);
                    }
                    else if (typeof(IScopedService).IsAssignableFrom(type))
                    {
                        services.AddScoped(interfaceType, type);
                    }
                }
            }
        }
    }
}
