namespace MarketData.Domain.ValueObjects;

using CsvHelper.Configuration.Attributes;
using System;

public class AssetCourse
{
    [Name("Date")]
    public DateTime Date { get; set; }

    [Name("Open")]
    public double Open { get; set; }

    [Name("Close")]
    public double Close { get; set; }

    [Name("Volume")]
    public long? Volume { get; set; }
}