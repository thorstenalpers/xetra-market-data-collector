﻿namespace MarketData.Infrastructure.Tests.Services;
using MarketData.Application.Interfaces;
using MarketData.Application.Options;
using MarketData.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

[TestFixture]
[Category("UnitTests")]
public class YahooApiClientTests
{
    private MockRepository mockRepository;

    private Mock<ILogger<YahooWebScraper>> mockLogger;
    private Mock<IWebDriverPoolManager> mockWebDriverPoolManager;
    private Mock<IYahooCookieManager> mockYahooCookieManager;
    private Mock<IHttpClientFactory> mockHttpClientFactory;
    private Mock<IOptions<SeleniumOptions>> mockOptions;

    [SetUp]
    public void SetUp()
    {
        mockRepository = new MockRepository(MockBehavior.Default);

        mockLogger = mockRepository.Create<ILogger<YahooWebScraper>>();
        mockHttpClientFactory = mockRepository.Create<IHttpClientFactory>();
        mockOptions = mockRepository.Create<IOptions<SeleniumOptions>>();
        mockWebDriverPoolManager = mockRepository.Create<IWebDriverPoolManager>();
        mockYahooCookieManager = mockRepository.Create<IYahooCookieManager>();
    }

    private YahooWebScraper CreateYahooApiClient()
    {
        return new YahooWebScraper(
            mockLogger.Object,
            mockYahooCookieManager.Object,
            mockHttpClientFactory.Object,
            mockOptions.Object,
            mockWebDriverPoolManager.Object);
    }
}
