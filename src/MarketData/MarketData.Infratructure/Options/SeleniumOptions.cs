namespace MarketData.Infratructure.Options;

using System;

public class SeleniumOptions
{
    public const string SectionName = "Selenium";
    public Uri Url { get; set; }
    public bool UseRemoteDriver { get; set; }
    public int MaxParallelism { get; set; }
}