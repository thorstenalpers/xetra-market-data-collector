namespace MarketData.Application.Specifications;

using Ardalis.Specification;
using MarketData.Application.Entities;
using MarketData.Application.ValueObjects;

public class AssetRecordSpec_ByFirstDate : Specification<AssetRecord>
{
    public AssetRecordSpec_ByFirstDate(int assetId, DateTime firstDate, bool includeTracking = false)
    {
        Query
            .Where(e => e.AssetId == assetId && e.Date >= firstDate);
    }
}
