namespace MarketData.FunctionalTests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using MarketData.API;
using MarketData.API.Consumer;
using MarketData.Domain.Entities;
using MarketData.Domain.Repositories;
using MarketData.Infratructure.Services;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Shared.Events;

[TestFixture]
[Category("IntegrationTests")]
public class IntegrationTests
{
    private IServiceProvider _serviceProvider;

    [Test]
    public async Task Database_Assets_VerifyNoDuplicatedSymbols()
    {
        var assetRepository = _serviceProvider.GetService<IRepository<Asset>>();
        var assets = await assetRepository.ListAsync();
        var duplicateGroups = assets
            .GroupBy(person => person.Symbol)
            .Where(group => group.Count() > 1)
            .ToList();

        Assert.IsEmpty(duplicateGroups);
    }

    [Test]
    public async Task GetCourses_CoursesValid()
    {
        // Arrange
        var yahooApiClient = _serviceProvider.GetService<IYahooWebScraper>();
        DateTime startDate = DateTime.UtcNow.AddDays(-10).Date;
        DateTime endDate = DateTime.UtcNow.Date;

        // Act
        var courses = await yahooApiClient.GetCourses(
            "XD4.DE",
            startDate,
            endDate);

        // Assert
        Assert.IsNotEmpty(courses);
        Assert.IsTrue(courses.All(e => e.Close > 10 && e.Close < 300));
    }

    [Test]
    public async Task Run_CreateAssetsRequested()
    {
        var consumer = _serviceProvider.GetService<CreateAssetsConsumer>();
        var mockConsumeContext = new Mock<ConsumeContext<CreateAssetsRequested>>();
        mockConsumeContext.Setup(e => e.Message).Returns(new CreateAssetsRequested
        {
        });
        await consumer.Consume(mockConsumeContext.Object);
        Assert.Pass();
    }


    [Test]
    public async Task Run_CreateAssetRecordsRequested()
    {
        var consumer = _serviceProvider.GetService<CreateAssetRecordsConsumer>();

        var mockConsumeContext = new Mock<ConsumeContext<CreateAssetRecordsRequested>>();
        mockConsumeContext.Setup(e => e.Message).Returns(new CreateAssetRecordsRequested
        {
            StartDate = new DateTime(2018, 01, 01).Date,
            EndDate = DateTime.UtcNow.Date,
            AssetIds = new List<int> { 1 }
        });

        await consumer.Consume(mockConsumeContext.Object);
        Assert.Pass();
    }

    [Test]
    public async Task Send_CreateAssetRecordsRequested()
    {
        var publishEndpoint = _serviceProvider.GetService<IPublishEndpoint>();

        await publishEndpoint.Publish(new CreateAssetRecordsRequested
        {
            StartDate = new DateTime(2018, 01, 01).Date,
            EndDate = DateTime.UtcNow.Date,
            AssetIds = new List<int> { 297 }
        });

        Assert.Pass();
    }

    [SetUp]
    public void Setup()
    {
        var host = Program.CreateWebApplication();
        var scope = host.Services.CreateScope();
        _serviceProvider = scope.ServiceProvider;
    }
}