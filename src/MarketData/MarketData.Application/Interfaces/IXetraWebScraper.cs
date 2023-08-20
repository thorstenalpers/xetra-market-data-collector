namespace MarketData.Application.Interfaces;

using MarketData.Application.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IXetraWebScraper
{
    public Task<List<XetraCsvEntry>> GetTradableInstruments();
}
