namespace MarketData.Application.Services;

using Ardalis.GuardClauses;
using AutoMapper;
using MarketData.Application.Services.Interfaces;
using MarketData.Domain.Entities;
using MarketData.Domain.Repositories;
using MarketData.Domain.Services.Interfaces;
using MarketData.Domain.Specifications;
using MarketData.Domain.ValueObjects;
using MarketData.Infrastructure;
using MarketData.Infrastructure.Options;
using MarketData.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AssetsService : IAssetsService, IScopedService
{
    private readonly ILogger<AssetsService> _logger;
    private readonly IMapper _mapper;
    private readonly IXetraWebScraper _xetraWebScraper;
    private readonly IRepository<Asset> _assetRepository;
    private readonly SeleniumOptions _seleniumOptions;
    private readonly IYahooWebScraper _yahooWebScraper;
    private readonly IRepository<AssetRecord> _assetRecordRepository;

    public AssetsService(ILogger<AssetsService> logger,
        IMapper mapper,
        IOptions<SeleniumOptions> seleniumOptions,
        IYahooWebScraper yahooWebScraper,
        IMathService mathService,
        IXetraWebScraper xetraWebScraper,
        IRepository<Asset> assetRepository,
        IRepository<AssetMetric> assetMetricRepository,
        IRepository<AssetRecord> asssetRecordRepository)
    {
        _logger = logger;
        _yahooWebScraper = yahooWebScraper;
        _seleniumOptions = seleniumOptions.Value;
        _xetraWebScraper = xetraWebScraper;
        _mapper = mapper;
        _assetRepository = assetRepository;
        _assetRecordRepository = asssetRecordRepository;
    }

    public async Task CreateAssets()
    {
        var csvEntries = await _xetraWebScraper.GetTradableInstruments();
        Guard.Against.NullOrEmpty(csvEntries);

        var assets = _mapper.Map<List<Asset>>(csvEntries);
        Guard.Against.NullOrEmpty(assets);
        _logger.LogInformation($"{assets.Count} assets downloaded from xetra.com");
        assets = await EnrichWithMetaDataAsync(assets);
        Guard.Against.NullOrEmpty(assets);
        _logger.LogInformation($"MetaData for {assets.Count} assets found on yahoo.com");
        await StoreAssetsAsync(assets);
        _logger.LogInformation($"{assets.Count} assets are stored in the database");
    }

    private async Task<List<Asset>> EnrichWithMetaDataAsync(List<Asset> assets)
    {
        var result = new List<Asset>();
        var validAssets = assets.Where(e =>
                e.Currency == "EUR" &&
                !string.IsNullOrEmpty(e.Isin) &&
                !string.IsNullOrEmpty(e.Symbol)
            ).ToList();

        var symbolsAndTypes = validAssets.Select(e => (e.Symbol, e.Type)).ToList();

        var lsMetaDataBySymbol = await GetMetaDataBySymbolAsync(symbolsAndTypes);
        var assetsWithoutMetaData = validAssets
            .Where(e => !lsMetaDataBySymbol.Any(t => t.Symbol == e.Symbol))
            .ToList();
        var isinsAndTypesWithoutMetaData = assetsWithoutMetaData.Select(e => (e.Isin, e.Type)).ToList();
        _logger.LogInformation($"Found {lsMetaDataBySymbol.Count} metadata by symbol");

        var allMetaData = lsMetaDataBySymbol;
        foreach (var metaData in allMetaData)
        {
            var validAsset = validAssets.FirstOrDefault(e => e.Symbol == metaData.Symbol);
            var asset = _mapper.Map(metaData, validAsset);
            result.Add(asset);
        }
        return result;
    }

    public async Task StoreAssetsAsync(List<Asset> assets)
    {
        var records = await _assetRecordRepository.ListAsync();
        await _assetRecordRepository.DeleteRangeAsync(records);

        var existingAssets = await _assetRepository.ListAsync();
        await _assetRepository.DeleteRangeAsync(existingAssets);

        await _assetRepository.AddRangeAsync(assets);
    }

    private async Task<List<AssetMetaData>> GetMetaDataBySymbolAsync(List<(string Symbol, EAssetType TradingAssetType)> symbolsAndTypes)
    {
        var tasks = new List<Task<List<AssetMetaData>>>();
        var lsSymbolsAndTypes = symbolsAndTypes.SplitIntoEqualSizedChunks(_seleniumOptions.MaxParallelism);
        foreach (var items in lsSymbolsAndTypes)
        {
            tasks.Add(Task.Run(() =>
            {
                return _yahooWebScraper.GetMetaDataBySymbol(items);
            }));
        }
        var result = await Task.WhenAll(tasks);
        return result.SelectMany(list => list).ToList();
    }

    public async Task<int> DeleteAssets(int daysWithNoCourses)
    {
        var assetsToDelete = new List<Asset>();
        var assets = await _assetRepository.ListAsync();
        foreach (var asset in assets)
        {
            var startDate = DateTime.UtcNow.AddDays(-daysWithNoCourses).Date;
            var spec = new AssetRecordsAfterDateSpecification(asset.Id, startDate);
            var records = await _assetRecordRepository.ListAsync(spec);

            if (!records.Any())
                assetsToDelete.Add(asset);
        }
        var cnt = assetsToDelete.Count;
        foreach (var asset in assetsToDelete)
        {
            var spec = new AssetRecordsByIdSpecification(asset.Id);
            var recordsToDelete = await _assetRecordRepository.ListAsync(spec);
            await _assetRecordRepository.DeleteRangeAsync(recordsToDelete);
        }
        await _assetRepository.DeleteRangeAsync(assetsToDelete);
        _logger.LogInformation($"{cnt} assets without courses have been deleted");
        return cnt;
    }
}

