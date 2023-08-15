namespace MarketData.Domain.Specifications;

using Ardalis.Specification;
using MarketData.Domain.Entities;
using MarketData.Domain.ValueObjects;

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
