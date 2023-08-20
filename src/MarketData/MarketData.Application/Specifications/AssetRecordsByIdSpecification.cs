namespace MarketData.Application.Specifications;

using Ardalis.Specification;
using MarketData.Application.Entities;

public class AssetRecordsByIdSpecification : Specification<AssetRecord>
{
    public AssetRecordsByIdSpecification(int assetId)
    {
        Query
            .Where(e => e.AssetId == assetId);
    }
}
