namespace MarketData.Application.Extensions;

using MarketData.Application.Exceptions;
using System.Globalization;
using System.Text.RegularExpressions;

public static class Helper
{
    public static string ExchangeNameToIso(string boerse)
    {
        var boerseIso = boerse switch
        {
            "ASX" => "AUS",
            "Berlin" => "GER",
            "Brussels" => "BEL",
            "BSE" => "IND",
            "Cboe" => "USA",
            "CBOT" => "USA",
            "CCC" => "",
            "CCY" => "USA",
            "Chicago" => "USA",
            "CME" => "USA",
            "COMEX" => "USA",
            "DJI" => "USA",
            "Dusseldorf" => "GER",
            "Frankfurt" => "GER",
            "FTSE" => "GBR",
            "Hamburg" => "GER",
            "HKSE" => "HKG",
            "ICE" => "",
            "Jakarta" => "INA",
            "Johannesburg" => "RSA",
            "KSE" => "PAK",
            "Kuala" => "MAS",
            "MCE" => "ESP",
            "Mexico" => "MEX",
            "Munich" => "GER",
            "Nasdaq" => "USA",
            "NasdaqCM" => "USA",
            "NasdaqGM" => "USA",
            "NasdaqGS" => "USA",
            "NY" => "USA",
            "NYSE" => "USA",
            "NYSEArca" => "USA",
            "NZSE" => "AUS",
            "Osaka" => "JPN",
            "Other" => "",
            "Paris" => "FRA",
            "São" => "BRA",
            "Shanghai" => "CHN",
            "Shenzhen" => "CHN",
            "SNP" => "USA",
            "Stuttgart" => "GER",
            "Taiwan" => "TPE",
            "Tel" => "ISR",
            "Toronto" => "CAN",
            "XETRA" => "GER",
            "Zurich" => "SUI",
            _ => null
        };
        return boerseIso;
    }

    public static string ToHistoricalCourseUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        if (!url.Contains('?') || url.Contains("/history"))
        {
            return url;
        }

        return url.Split('?')[0] + "/history";
    }

    public static string ToLongString(this string str, string comma = ",")
    {
        //e.g. "1,07 Mrd." or "100.234,07 Mio."
        // e.g. "1.07" or "1.07 M EUR"

        var match = new Regex("([0-9.,]+)([A-Za-z]?)").Match(str);
        if (!match.Success)
        {
            return null;
        }

        var strValue = match.Groups[1].Value;

        if (comma == ",")
        {
            strValue = strValue.Replace(".", ""); // Vorkommastellen
        }

        if (comma == ".")
        {
            strValue = strValue.Replace(",", "");   // Vorkommastellen
        }

        if (comma == ",")
        {
            strValue = strValue.Replace(",", ".");
        }

        var result = double.Parse(strValue, CultureInfo.InvariantCulture);
        if (match.Groups.Count <= 2 || string.IsNullOrEmpty(match.Groups[2].Value))
        {
            return $"{(long)result}";
        }

        var strMultiplicator = match.Groups[2].Value;
        if (strMultiplicator == "Trl." || strMultiplicator == "Trl" || strMultiplicator == "T")
        {
            result *= 1000000000000000;
        }
        else if (strMultiplicator == "Bio." || strMultiplicator == "Bio" || strMultiplicator == "B")
        {
            result *= 1000000000000;
        }
        else if (strMultiplicator == "Mrd." || strMultiplicator == "Mrd")
        {
            result *= 1000000000;
        }
        else if (strMultiplicator == "Mio." || strMultiplicator == "Mio" || strMultiplicator == "M")
        {
            result *= 1000000;
        }
        else if (strMultiplicator == "k" || strMultiplicator == "K")
        {
            result *= 1000;
        }
        else
        {
            throw new MarketDataException($"Unknown multiplikator={strMultiplicator}");
        }
        var strResult = $"{(long)result}";
        if (strResult.EndsWith("99", StringComparison.InvariantCultureIgnoreCase))
        {
            return $"{(long)(result + 1)}";
        }
        return $"{(long)result}";
    }


    public static long ToUnixTimeSeconds(this DateTime dateTime)
    {
        return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
    }
}
