namespace EventPublisher;

using System.Reflection;
using MassTransit;
using Microsoft.OpenApi.Models;
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
using EventPublisher.Options;

public class Program
{
    public static void Main()
    {
        var app = CreateWebApplication();
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
        services.AddCors();
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
        services.AddEndpointsApiExplorer();
        services.AddHealthChecks();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Market Data Collector",
            });
            opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            opt.UseInlineDefinitionsForEnums();
        });
        services.AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SectionName))
            .ValidateDataAnnotations();

        services.AddHttpClient();

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
        app.UseSwagger();
        app.UseSwaggerUI(config =>
        {
            config.DocumentTitle = "Market Data Collector";
        });
        app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials()
            );
        app.UseAuthorization();

        app.MapControllers();
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

