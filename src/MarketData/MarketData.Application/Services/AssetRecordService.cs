namespace MarketData.Application.Services;

using Ardalis.GuardClauses;
using MarketData.Application.Services.Interfaces;
using MarketData.Domain.Entities;
using MarketData.Domain.Exceptions;
using MarketData.Domain.Repositories;
using MarketData.Domain.Services.Interfaces;
using MarketData.Domain.Specifications;
using MarketData.Infratructure.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AssetRecordService : IAssetRecordService, IScopedService
{
    private readonly ILogger<AssetRecordService> _logger;
    private readonly IRepository<Asset> _assetRepository;
    private readonly IYahooWebScraper _yahooWebScraper;
    private readonly ICourseMetricsCalculator _courseMetricsCalculator;
    private readonly IRepository<AssetRecord> _assetRecordRepository;


    public AssetRecordService(ILogger<AssetRecordService> logger,
        ICourseMetricsCalculator courseMetricsCalculator,
        IYahooWebScraper yahooWebScraper,
        IRepository<Asset> assetRepository,
        IRepository<AssetRecord> assetRecordRepository)
    {
        _logger = logger;
        _yahooWebScraper = yahooWebScraper;
        _courseMetricsCalculator = courseMetricsCalculator;
        _assetRepository = assetRepository;
        _assetRecordRepository = assetRecordRepository;
    }

    public async Task CreateRecords(List<int> assetIds, DateTime startDate, DateTime? endDate)
    {
        Guard.Against.NullOrEmpty(assetIds);

        if (endDate == null) endDate = DateTime.UtcNow.Date;
        var tasks = new List<Task>();

        var assets = await _assetRepository.ListAsync();
        if (assetIds != null && assetIds.Any())
        {
            assets = assets.Where(e => assetIds.Contains(e.Id)).ToList();
        }
        int index = 0;
        foreach (var asset in assets)
        {
            var courses = await _yahooWebScraper.GetCourses(asset.Symbol, startDate, endDate);
            if (courses == null || !courses.Any())
                continue;
            var records = await _courseMetricsCalculator.TransformToRecordsAsync(asset, courses, startDate);
            try
            {
                var spec = new AssetRecordsAfterDateSpecification(asset.Id, startDate);
                var existingRecords = await _assetRecordRepository.ListAsync(spec);
                await _assetRecordRepository.DeleteRangeAsync(existingRecords);

                var addedRecords = new List<AssetRecord>();
                await _assetRecordRepository.AddRangeAsync(records);
            }
            catch (Exception ex)
            {
                throw new MarketDataException($"Failed to update the database for {asset.Symbol}", ex);
            }
            _logger.LogInformation($"{index + 1} / {assets.Count}: Added {records.Count} records for {asset.Symbol}");
            index++;
        }
        await Task.WhenAll(tasks);
    }
}

