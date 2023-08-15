namespace MarketData.Domain.Specifications;

using Ardalis.Specification;
using MarketData.Domain.Entities;

public class AssetRecordsByIdSpecification : Specification<AssetRecord>
{
    public AssetRecordsByIdSpecification(int assetId)
    {
        Query
            .Where(e => e.AssetId == assetId);
    }
}
