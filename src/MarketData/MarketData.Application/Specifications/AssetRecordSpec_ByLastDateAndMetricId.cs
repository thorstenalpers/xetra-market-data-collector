namespace MarketData.Application.Specifications;

using Ardalis.Specification;
using MarketData.Application.Entities;
using MarketData.Application.ValueObjects;
using System.Linq.Expressions;

public class AssetRecordSpec_ByLastDateAndMetricId : Specification<AssetRecord>
{
    public AssetRecordSpec_ByLastDateAndMetricId(int assetId, EAssetMetricType assetMetricType, DateTime maxDate, bool includeTracking = false)
    {
        Expression<Func<AssetRecord, bool>> condition = e => e.AssetId == assetId && e.AssetMetricId == (int)assetMetricType && e.Date <= maxDate;
        if (includeTracking)
            Query.Where(condition).OrderByDescending(e => e.Date);
        else
            Query.AsNoTracking().Where(condition).OrderByDescending(e => e.Date);
    }
}
