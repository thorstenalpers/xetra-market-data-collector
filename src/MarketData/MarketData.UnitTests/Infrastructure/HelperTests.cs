namespace MarketData.UnitTests.Infrastructure;
using MarketData.Infratructure.Services;
using NUnit.Framework;

[TestFixture]
[Category("UnitTests")]
public class HelperTests
{

    [TestCase("https://de.finance.yahoo.com/quote/UNI7083-USD?p=UNI7083-USD&.tsrc=fin-srch", "https://de.finance.yahoo.com/quote/UNI7083-USD/history")]
    [TestCase("https://de.finance.yahoo.com/quote/UNI7083-USD/history", "https://de.finance.yahoo.com/quote/UNI7083-USD/history")]
    [TestCase("https://de.finance.yahoo.com/quote/SAP.DE?p=SAP.DE&.tsrc=fin-srch", "https://de.finance.yahoo.com/quote/SAP.DE/history")]
    [TestCase(null, null)]
    [TestCase("", null)]
    [TestCase(null, null)]
    public void Helper_ToHistoricalCourseUrl(string url, string expectedUrl)
    {
        var result = Helper.ToHistoricalCourseUrl(url);

        Assert.AreEqual(expectedUrl, result);
    }

    [TestCase("765,36k", "765360")]
    public void Helper_ToLongString(string strValue, string expectedLongString)
    {
        var result = strValue.ToLongString();

        Assert.AreEqual(expectedLongString, result);
    }
}
