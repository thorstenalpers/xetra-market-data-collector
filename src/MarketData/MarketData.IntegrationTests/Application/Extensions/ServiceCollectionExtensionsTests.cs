using AutoMapper;
using MarketData.API.Extensions;
using MarketData.Application.Entities;
using MarketData.Application.Interfaces;
using MarketData.Application.Options;
using MarketData.Application.Repositories;
using MarketData.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Net.Http;

namespace MarketData.IntegrationTests.Application.Extensions;

[TestFixture]
[Category("UnitTests")]
public class ServiceCollectionExtensionsTests
{
    [Test]
    public void AddRepositories_Success()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new DbContextOptionsBuilder<MarketDataDbContext>()
            .UseInMemoryDatabase(databaseName: "TradeX")
            .Options;
        services.AddScoped(_ => new Mock<DbContextOptions<MarketDataDbContext>>().Object);
        services.AddScoped(_ => new Mock<MarketDataDbContext>(options).Object);

        // Act
        services.AddRepositories();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var repository = serviceProvider.GetRequiredService<IRepository<Asset>>();
        Assert.NotNull(repository);
    }

    [Test]
    public void AddApplicationServices_Success()
    {
        // Arrange
        var services = new ServiceCollection();
        var options = new DbContextOptionsBuilder<MarketDataDbContext>()
            .UseInMemoryDatabase(databaseName: "TradeX")
            .Options;
        services.AddDomainServices();
        services.AddInfrastructureServices();
        services.AddRepositories();
        services.AddLogging();
        services.AddScoped(_ => new Mock<MarketDataDbContext>(options).Object);
        services.AddScoped(_ => new Mock<IMapper>().Object);
        services.AddScoped(_ => new Mock<IWebDriverPoolManager>().Object);
        services.AddScoped(_ => new Mock<IHttpClientFactory>().Object);
        services.AddScoped(_ => new Mock<IOptions<SeleniumOptions>>().Object);

        // Act
        services.AddApplicationServices();


        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<IAssetsService>();
        Assert.NotNull(service);
    }

    //[Test]
    //public void AddDomainServices_Success()
    //{
    //    // Arrange
    //    var services = new ServiceCollection();
    //    var options = new DbContextOptionsBuilder<TradeXDbContext>()
    //        .UseInMemoryDatabase(databaseName: "TradeX")
    //        .Options;
    //    services.AddRepositories();
    //    services.AddInfrastructureServices();
    //    services.AddLogging();
    //    services.AddScoped(_ => new Mock<IWebDriverPoolManager>().Object);
    //    services.AddScoped(_ => new Mock<TradeXDbContext>(options).Object);
    //    services.AddScoped(_ => new Mock<IHttpClientFactory>().Object);
    //    services.AddScoped(_ => new Mock<IMapper>().Object);
    //    services.AddScoped(_ => new Mock<IOptions<SeleniumOptions>>().Object);

    //    // Act
    //    services.AddDomainServices();


    //    // Assert
    //    var serviceProvider = services.BuildServiceProvider();
    //    var service = serviceProvider.GetRequiredService<IInstrumentDomainService>();
    //    Assert.NotNull(service);
    //}

    [Test]
    public void AddInfrastructureServices_Added()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped(_ => new Mock<IHttpClientFactory>().Object);
        services.AddScoped(_ => new Mock<IOptions<XetraOptions>>().Object);

        // Act
        services.AddInfrastructureServices();

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var xetraApiClient = serviceProvider.GetRequiredService<IXetraWebScraper>();
        Assert.NotNull(xetraApiClient);
    }
}
