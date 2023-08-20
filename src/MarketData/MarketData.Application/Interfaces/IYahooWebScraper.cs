namespace MarketData.Application.Interfaces;

using MarketData.Application.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IYahooWebScraper
{
    List<AssetMetaData> GetMetaDataBySymbol(List<(string symbol, EAssetType type)> symbolsAndTypes);
    Task<List<AssetCourse>> GetCourses(string symbol, DateTime startDate, DateTime? endDate = null);
    List<(string isin, string symbol)> GetSymbolsByIsin(List<string> isins);
}
