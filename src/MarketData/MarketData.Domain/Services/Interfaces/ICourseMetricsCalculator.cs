namespace MarketData.Domain.Services.Interfaces;
using MarketData.Domain.Entities;

using MarketData.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICourseMetricsCalculator
{
    Task<List<AssetRecord>> TransformToRecordsAsync(Asset asset, List<AssetCourse> courses, DateTime startDate);
}
