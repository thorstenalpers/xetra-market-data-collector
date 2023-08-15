namespace MarketData.Domain.ValueObjects;
public class AssetMetaData
{
    public string Symbol { get; set; }
    public string Name { get; set; } = "";
    public string Url { get; set; }

    public string Industry { get; set; }
    public string Sector { get; set; }
    public long? AvgTradingVolume { get; set; }
    public long? MarketCapitalization { get; set; }
    public long? CntEmployees { get; set; }
    public string Exchange { get; set; }
    public string ExchangeCountryIso { get; set; }
    public string ExchangeCloseTime { get; set; }
    public string Currency { get; set; }
}
