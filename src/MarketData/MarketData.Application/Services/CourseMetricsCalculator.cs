namespace MarketData.Application.Services;
using Ardalis.GuardClauses;
using MarketData.Application.Entities;
using MarketData.Application.Interfaces;
using MarketData.Application.Repositories;
using MarketData.Application.Specifications;
using MarketData.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

public class CourseMetricsCalculator : ICourseMetricsCalculator, IScopedService
{
    public List<AssetRecord> TransformToRecordsAsync(Asset asset, List<AssetCourse> courses, DateTime startDate)
    {
        Guard.Against.Null(asset);
        Guard.Against.NullOrEmpty(courses);
        var recordsToAdd = new List<AssetRecord>();

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
