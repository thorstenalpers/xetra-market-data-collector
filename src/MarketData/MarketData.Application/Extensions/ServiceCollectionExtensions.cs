namespace MarketData.Application.Extensions;

using MarketData.Application.Services.Interfaces;
using MarketData.Domain.Repositories;
using MarketData.Domain.Services.Interfaces;
using MarketData.Infratructure;
using MarketData.Infratructure.Services;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
    }

    public static void AddApplicationServices(this IServiceCollection services)
    {
        var assembly = typeof(IAssetsService).Assembly;
        var appNamespace = typeof(IAssetsService).Namespace;
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

    public static void AddDomainServices(this IServiceCollection services)
    {
        var assembly = typeof(IMathService).Assembly;
        var appNamespace = typeof(IMathService).Namespace;
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

    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        var assembly = typeof(YahooWebScraper).Assembly;
        var appNamespace = typeof(YahooWebScraper).Namespace;
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
