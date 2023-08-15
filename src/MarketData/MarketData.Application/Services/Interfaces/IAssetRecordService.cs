namespace MarketData.Application.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IAssetRecordService
{
    Task CreateRecords(List<int> assetIds, DateTime startDate, DateTime? endDate);
}