namespace MarketData.Application.Specifications;

using Ardalis.Specification;
using MarketData.Application.Entities;
using MarketData.Application.ValueObjects;

public class LastAssetRecordSpecification : Specification<AssetRecord>, ISingleResultSpecification<AssetRecord>
{
    public LastAssetRecordSpecification(int assetId, EAssetMetricType assetMetricType, DateTime maxDate)
    {
        Query
            .Where(e => e.AssetId == assetId && e.AssetMetricId == (int)assetMetricType)
            .Where(e => e.Date <= maxDate)
            .OrderByDescending(e => e.Date);
    }
}
