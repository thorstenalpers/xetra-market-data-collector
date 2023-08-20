namespace MarketData.API;

using System.Reflection;
using MassTransit;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using Microsoft.Extensions.Options;
using MarketData.Application.Repositories;
using MarketData.Application.Options;
using MarketData.API.Extensions;
using MarketData.Infrastructure.Mappings;
using MarketData.Infrastructure;
using MarketData.Infrastructure.Repositories;
using MarketData.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

public class Program
{
    public static void Main()
    {
        var app = CreateWebApplication();
        using (var scope = app.Services.CreateScope())
        {
            var poolManager = scope.ServiceProvider.GetService<IWebDriverPoolManager>();
            app.Lifetime.ApplicationStopping.Register(poolManager.Dispose);
        }
        app.Run();
    }

    public static WebApplication CreateWebApplication()
    {
        var builder = WebApplication.CreateBuilder();

        var configuration = builder.Configuration;
        var services = builder.Services;
        builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message:l}{NewLine}{Exception}"));

        builder.WebHost.ConfigureKestrel(option =>
        {
            option.Limits.KeepAliveTimeout = TimeSpan.FromHours(10);
            option.Limits.RequestHeadersTimeout = TimeSpan.FromHours(10);
        });
        //services.AddCors();
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            Converters = new List<Newtonsoft.Json.JsonConverter> { new StringEnumConverter() },
            Formatting = Formatting.None,
            DateFormatString = "yyyy-MM-dd",
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        services.AddHealthChecks();

        services.AddAutoMapper(typeof(AssetMetaDataProfile).Assembly);
        services.AddOptions<SeleniumOptions>()
            .Bind(configuration.GetSection(SeleniumOptions.SectionName))
            .ValidateDataAnnotations();
        services.AddOptions<XetraOptions>()
            .Bind(configuration.GetSection(XetraOptions.SectionName))
            .ValidateDataAnnotations();
        services.AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SectionName))
            .ValidateDataAnnotations();

        services.AddHttpClient();
        services.AddMemoryCache();

        builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        builder.Services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

        services.AddApplicationServices();
        services.AddInfrastructureServices();
        services.AddDomainServices();

        services.AddDbContext<MarketDataDbContext>(
            dbContextOptions =>
            {
                dbContextOptions
                    .UseLazyLoadingProxies()
                    .UseNpgsql(configuration.GetConnectionString("TradeX"), builder =>
                    {
                        builder.EnableRetryOnFailure(32, TimeSpan.FromSeconds(30), null);
                        builder.CommandTimeout(100000);
                        builder.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "TradeX");
                    });
            });

        services.AddMassTransit(busCfg =>
        {
            busCfg.AddConsumers(Assembly.GetExecutingAssembly());
            busCfg.SetKebabCaseEndpointNameFormatter();
            busCfg.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetService<IOptions<RabbitMqOptions>>().Value;

                cfg.UseConcurrencyLimit(5);
                cfg.PrefetchCount = 5;
                cfg.Host(host: options.AmqpUri, h =>
                {
                    h.Username(options.Username);
                    h.Password(options.Password);
                });
                cfg.ConfigureEndpoints(context);
            });
        });
        WebApplication app = builder.Build();
        app.UseDeveloperExceptionPage();

        //app.UseCors(builder => builder
        //        .AllowAnyHeader()
        //        .AllowAnyMethod()
        //        .SetIsOriginAllowed((host) => true)
        //        .AllowCredentials()
        //    );
        //app.UseAuthorization();

        //app.MapControllers();
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true
        });

        app.Use(async (context, next) =>
        {
            try
            {
                await next(context);
            }
            catch (TaskCanceledException)
            {
                // if client closes exception, ignore Exception and return a proper HTTP statusCode 
                context.Response.StatusCode = 499;  // Client Closed Request
            }
            catch (Exception ex)
            {
                // if client closes exception, ignore Exception and return a proper HTTP statusCode 
                context.Response.StatusCode = 499;  // Client Closed Request
                Console.WriteLine($"Exception {ex}, {ex?.InnerException}");
            }
        });
        return app;
    }
}

