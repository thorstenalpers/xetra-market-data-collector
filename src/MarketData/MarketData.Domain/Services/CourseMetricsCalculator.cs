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
    public List<AssetRecord> TransformToRecordsAsync(Asset asset, List<AssetCourse> courses, DateTime startDate)
    {
        Guard.Against.Null(asset);
        Guard.Against.NullOrEmpty(courses);
        var recordsToAdd = new List<AssetRecord>();

        var spec = new LastAssetRecordSpecification(asset.Id, EAssetMetricType.PriceClose, startDate.AddDays(-1).Date);

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

            if (course.Volume != null)
            {
                var volumen = new AssetRecord
                {
                    Date = course.Date,
                    Value = course.Volume.Value,
                    AssetId = asset.Id,
                    AssetMetricId = (int)EAssetMetricType.Volumen,
                };
                recordsToAdd.Add(volumen);
            }
        }
        return recordsToAdd;
    }
}
