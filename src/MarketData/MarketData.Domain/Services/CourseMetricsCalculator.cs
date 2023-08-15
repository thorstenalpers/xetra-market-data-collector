namespace MarketData.Domain.Services;
using Ardalis.GuardClauses;
using MarketData.Domain.Entities;
using MarketData.Domain.Repositories;
using MarketData.Domain.Services.Interfaces;
using MarketData.Domain.Specifications;

using MarketData.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class CourseMetricsCalculator : ICourseMetricsCalculator, IScopedService
{
    private readonly IMathService _mathService;
    private readonly IRepository<AssetRecord> _assetRecordRepository;
    public CourseMetricsCalculator(IMathService mathService, IRepository<AssetRecord> assetRecordRepository)
    {
        _mathService = mathService;
        _assetRecordRepository = assetRecordRepository;
    }
    public async Task<List<AssetRecord>> TransformToRecordsAsync(Asset asset, List<AssetCourse> courses, DateTime startDate)
    {
        Guard.Against.Null(asset);
        Guard.Against.NullOrEmpty(courses);
        var recordsToAdd = new List<AssetRecord>();

        var spec = new LastAssetRecordSpecification(asset.Id, EAssetMetricType.PriceClose, startDate.AddDays(-1).Date);
        var lastClosePriceRecord = await _assetRecordRepository.FirstOrDefaultAsync(spec);
        var lastClosePrice = lastClosePriceRecord?.Value;

        courses = courses.OrderBy(e => e.Date).ToList();

        foreach (var course in courses)
        {
            var openPrice = new AssetRecord
            {
                Date = course.Date,
                Value = course.Close,
                AssetId = asset.Id,
                AssetMetricId = (int)EAssetMetricType.PriceOpen,
            };
            var closePrice = new AssetRecord
            {
                Date = course.Date,
                Value = course.Open,
                AssetId = asset.Id,
                AssetMetricId = (int)EAssetMetricType.PriceClose,
            };
            recordsToAdd.Add(openPrice);
            recordsToAdd.Add(closePrice);

            var dp1_OpenClose = _mathService.CalculateDeltaPercentage(openPrice?.Value, closePrice?.Value) ?? 0;
            var dp1_CloseClose = _mathService.CalculateDeltaPercentage(lastClosePrice, closePrice?.Value) ?? 0;

            var dp1OpenClose = new AssetRecord
            {
                Date = course.Date,
                Value = dp1_OpenClose,
                AssetId = asset.Id,
                AssetMetricId = (int)EAssetMetricType.PriceDeltaPercentage_PriceOpenToPriceClose_1Day,
            };

            var dp1CloseClose = new AssetRecord
            {
                Date = course.Date,
                Value = dp1_CloseClose,
                AssetId = asset.Id,
                AssetMetricId = (int)EAssetMetricType.PriceDeltaPercentage_PriceCloseToPriceClose_1Day,
            };
            recordsToAdd.Add(dp1OpenClose);
            recordsToAdd.Add(dp1CloseClose);
            lastClosePrice = closePrice.Value;
        }
        return recordsToAdd;
    }
}
