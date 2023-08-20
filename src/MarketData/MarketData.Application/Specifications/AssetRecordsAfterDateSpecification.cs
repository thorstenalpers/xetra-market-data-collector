namespace MarketData.Application.Specifications;

using Ardalis.Specification;
using MarketData.Application.Entities;

//public class StoreNamesSpec : Specification<AssetRecordEntity, string>
//{
//    public StoreNamesSpec()
//    {
//        Query.Select(x => x.Date.ToString());
//    }
//}

public class AssetRecordsAfterDateSpecification : Specification<AssetRecord>
{
    public AssetRecordsAfterDateSpecification(int assetId, DateTime startDate)
    {
        Query
            .Where(e => e.AssetId == assetId && e.Date >= startDate);
    }
}
