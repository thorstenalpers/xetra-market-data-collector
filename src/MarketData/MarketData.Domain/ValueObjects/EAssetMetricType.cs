namespace MarketData.Domain.ValueObjects;

public enum EAssetMetricType : int
{
    PriceClose = 1,
    PriceOpen = 2,
    //Volumen = 3,

    PriceDeltaPercentage_PriceCloseToPriceClose_1Day = 101,
    //PriceDeltaPercentage_PriceCloseToPriceClose_2Days = 102,
    //PriceDeltaPercentage_PriceCloseToPriceClose_3Days = 103,
    //PriceDeltaPercentage_PriceCloseToPriceClose_4Days = 104,
    //PriceDeltaPercentage_PriceCloseToPriceClose_5Days = 105,
    //PriceDeltaPercentage_PriceCloseToPriceClose_10Days = 110,
    //PriceDeltaPercentage_PriceCloseToPriceClose_20Days = 120,
    //PriceDeltaPercentage_PriceCloseToPriceClose_50Days = 150,
    //PriceDeltaPercentage_PriceCloseToPriceClose_99Days = 199,

    PriceDeltaPercentage_PriceOpenToPriceClose_1Day = 201,

    //VolumenDeltaPercentage_1Day = 301,
    //VolumenDeltaPercentage_2Days = 302,
    //VolumenDeltaPercentage_3Days = 303,
}