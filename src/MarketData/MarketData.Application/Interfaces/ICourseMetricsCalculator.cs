namespace MarketData.Application.Interfaces;

using MarketData.Application.Entities;
using MarketData.Application.ValueObjects;
using System;
using System.Collections.Generic;

public interface ICourseMetricsCalculator
{
    List<AssetRecord> TransformToRecordsAsync(Asset asset, List<AssetCourse> courses, DateTime startDate);
}
