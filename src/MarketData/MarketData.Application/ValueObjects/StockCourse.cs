namespace MarketData.Application.ValueObjects;

using CsvHelper.Configuration.Attributes;

public class YahooCourseStringValues
{
    [Name("Date")]
    public string Date { get; set; }

    [Name("Open")]
    public string Open { get; set; }

    [Name("Close")]
    public string Close { get; set; }

    [Name("Volume")]
    public string Volume { get; set; }
}