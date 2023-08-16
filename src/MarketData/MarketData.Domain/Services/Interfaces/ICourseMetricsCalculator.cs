namespace MarketData.Domain.Services.Interfaces;
using MarketData.Domain.Entities;

using MarketData.Domain.ValueObjects;
using System;
using System.Collections.Generic;

public interface ICourseMetricsCalculator
{
    List<AssetRecord> TransformToRecordsAsync(Asset asset, List<AssetCourse> courses, DateTime startDate);
}
