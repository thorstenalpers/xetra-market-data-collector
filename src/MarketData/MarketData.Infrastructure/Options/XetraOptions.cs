namespace MarketData.Infrastructure.Options;

using System;

public class XetraOptions
{
    public const string SectionName = "Xetra";
    public Uri DownloadUrl { get; set; }
}
