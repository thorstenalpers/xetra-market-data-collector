namespace MarketData.Application.Entities;
using MarketData.Application.ValueObjects;

public class Asset : BaseEntity
{
    public string Symbol { get; set; } = "";
    public EAssetType Type { get; set; }
    public string Name { get; set; } = "";

    public string Isin { get; set; } = "";
    public string Mnemonic { get; set; } = "";

    public string Currency { get; set; }

    public string Url { get; set; }

    public string Industry { get; set; }
    public string Sector { get; set; }
    public long? AvgTradingVolume { get; set; }
    public long? MarketCapitalization { get; set; }
    public long? CntEmployees { get; set; }
    public string Exchange { get; set; }
    public string ExchangeCountryIso { get; set; }
    public string ExchangeCloseTime { get; set; }
}
