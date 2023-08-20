namespace MarketData.Application.Specifications;

using Ardalis.Specification;
using MarketData.Application.Entities;
using MarketData.Application.ValueObjects;

public class AssetRecordSpec_ByAssetId : Specification<AssetRecord>
{
    public AssetRecordSpec_ByAssetId(int assetId, bool includeTracking = false)
    {
        Query
            .Where(e => e.AssetId == assetId);
    }
}
