namespace MarketData.Infratructure.Services;

using CsvHelper;
using MarketData.Domain.Exceptions;
using MarketData.Domain.Repositories;
using MarketData.Domain.ValueObjects;
using MarketData.Infratructure.Options;
using MarketData.Infratructure.Services.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Polly;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

public interface IYahooWebScraper
{
    List<AssetMetaData> GetMetaDataBySymbol(List<(string symbol, EAssetType type)> symbolsAndTypes);
    Task<List<AssetCourse>> GetCourses(string symbol, DateTime startDate, DateTime? endDate = null);
}

public class YahooWebScraper : IYahooWebScraper, IScopedService
{
    private readonly object _lock = new object();
    private readonly ILogger<YahooWebScraper> _logger;
    private readonly HttpClient _httpClient;
    private readonly SeleniumOptions _seleniumOptions;
    private readonly IYahooCookieManager _yahooCookieManager;
    private readonly IWebDriverPoolManager _webDriverPoolManager;

    public YahooWebScraper(ILogger<YahooWebScraper> logger,
        IYahooCookieManager yahooCookieManager, IHttpClientFactory httpClientFactory, IOptions<SeleniumOptions> options,
        IWebDriverPoolManager webDriverPoolManager)
    {
        _logger = logger;
        _yahooCookieManager = yahooCookieManager;
        _httpClient = httpClientFactory.CreateClient(nameof(XetraWebScraper));
        _seleniumOptions = options.Value;

        _webDriverPoolManager = webDriverPoolManager;
    }

    public List<AssetMetaData> GetMetaDataBySymbol(List<(string symbol, EAssetType type)> symbolsAndTypes)
    {
        var result = new List<AssetMetaData>();
        IWebDriver driver = null;
        WebDriverWait wait = null;
        var policy = Policy.Handle<Exception>()
            .WaitAndRetry(3, retryAttempt => TimeSpan.FromMilliseconds(500));

        try
        {
            driver = _webDriverPoolManager.GetDriver(false, _seleniumOptions.UseRemoteDriver, _seleniumOptions.Url);
            wait = _webDriverPoolManager.CreateWebDriverWait(driver, TimeSpan.FromSeconds(3));

            driver.Navigate().GoToUrl("https://de.finance.yahoo.com");
            driver.WaitForPageReady(true);
            try
            {
                var privacyButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[@name='agree']")));
                privacyButton.Click();
                driver.SwitchTo().ParentFrame();
                driver.WaitForPageReady();
            }
            catch { }

            foreach (var symbolAndType in symbolsAndTypes)
            {
                try
                {
                    policy.Execute(() =>
                    {
                        var url = $"https://de.finance.yahoo.com/quote/{symbolAndType.symbol}?p={symbolAndType.symbol}&.tsrc=fin-srch";
                        try
                        {
                            driver.WaitForPageReady(true);
                            driver.Navigate().GoToUrl("https://de.finance.yahoo.com");
                            driver.WaitForPageReady(true);
                            driver.WaitForPageReady(true);
                            driver.WaitForPageReady(true);
                            driver.Navigate().GoToUrl(url);
                            driver.WaitForPageReady(true);
                            driver.WaitForPageReady(true);
                            wait.Until(ExpectedConditions.UrlMatches(".+de.finance.yahoo.com/.+"));
                            url = driver.Url;

                            if (url.Contains("lookup", StringComparison.InvariantCultureIgnoreCase))
                            {
                                wait.Until(ExpectedConditions.ElementToBeClickable(
                                    By.XPath($"//a[@data-symbol='{symbolAndType.symbol}' and contains(@href, '/quote/')]")))
                                .Click();
                                driver.WaitForPageReady(true);
                                driver.WaitForPageReady(true);
                                driver.WaitForPageReady(true);
                                wait.Until(ExpectedConditions.UrlMatches(".+de.finance.yahoo.com/quote.+"));
                                driver.WaitForPageReady(true);
                                driver.WaitForPageReady(true);
                                driver.WaitForPageReady(true);
                            }
                        }
                        catch
                        {
                            _logger.LogWarning($"Skipped: No results for {symbolAndType.symbol}");
                            //tradingAssetsWithoutMetaData.Add(tradingAsset);
                            // check the symbol / description on finanzen.net, remove exchange in the last letter 
                            return;
                        }
                        var strName = driver.GetText(By.XPath("//h1"), wait);
                        var match = new Regex("(.+) [(].+[)]").Match(strName);
                        if (match.Success)
                        {
                            strName = match.Groups[1].Value;
                        }

                        var strBoersenSchluss = driver.GetText(By.XPath("//span[contains(., 'Börsenschluss')]"), wait);
                        var strBoerseAndCurrency = driver.GetText(By.XPath("//span[contains(text(), 'Währung in ')]"), wait);
                        var bufferBoerseAndCurrency = strBoerseAndCurrency?.Split(" ");
                        strBoersenSchluss = strBoersenSchluss?.Replace("Börsenschluss: ", "");

                        var boerse = bufferBoerseAndCurrency?.FirstOrDefault();
                        var boerseIso = Helper.ExchangeNameToIso(boerse);

                        var currency = bufferBoerseAndCurrency?.LastOrDefault();

                        var strAverageVolume = driver.GetText(By.XPath("//td/span[contains(text(),'Durchschn. Volumen')]/../following-sibling::td[1]"), wait);
                        long? avgTradingVolume = !string.IsNullOrWhiteSpace(strAverageVolume) && strAverageVolume != "-" ?
                                long.Parse(strAverageVolume.ToLongString(","), CultureInfo.InvariantCulture) : null;

                        long? mitarbeiter = null, nettovermoegen = null, marktkapitalisierung = null;
                        string sektor = null, branche = null;

                        if (symbolAndType.type == EAssetType.ETF)
                        {
                            var strNettovermoegen = driver.GetText(By.XPath("//td/span[contains(text(),'Nettovermögen')]/../following-sibling::td[1]"), wait);

                            nettovermoegen = !string.IsNullOrWhiteSpace(strNettovermoegen) && strNettovermoegen != "-" ?
                                long.Parse(strNettovermoegen.ToLongString(","), CultureInfo.InvariantCulture) : null;

                            var found = driver.RetryClick(By.XPath("//a/span[text()='Profil']/.."), wait, null, false, 1);
                            driver.WaitForPageReady(true);

                            sektor = driver.GetText(By.XPath("//span/span[text()='Kategorie']/../../following-sibling::span[1]"), wait);
                        }
                        if (symbolAndType.type == EAssetType.Stock)
                        {
                            var strMarktkapitalisierung = driver.GetText(By.XPath("//td/span[contains(text(),'Marktkap')]/../following-sibling::td[1]"), wait);

                            marktkapitalisierung = !string.IsNullOrWhiteSpace(strMarktkapitalisierung) && strMarktkapitalisierung != "-" ?
                                long.Parse(strMarktkapitalisierung.ToLongString(","), CultureInfo.InvariantCulture) : null;


                            var found = driver.RetryClick(By.XPath("//a/span[text()='Profil']/.."), wait, null, false, 1);
                            driver.WaitForPageReady(true);

                            var strMitarbeiter = driver.GetText(By.XPath("//p/span[contains(text(),'itarbeiter')]/following-sibling::span[1]"), wait);
                            sektor = driver.GetText(By.XPath("//p/span[text()='Sektor(en)']/following-sibling::span[1]"), wait);
                            branche = driver.GetText(By.XPath("//p/span[text()='Branche']/following-sibling::span[1]"), wait);

                            mitarbeiter = !string.IsNullOrWhiteSpace(strMitarbeiter) && strMitarbeiter != "-" ?
                                long.Parse(strMitarbeiter.ToLongString(","), CultureInfo.InvariantCulture) : null;
                        }
                        var metaData = new AssetMetaData
                        {
                            Exchange = boerse,
                            ExchangeCountryIso = boerseIso,
                            ExchangeCloseTime = strBoersenSchluss,
                            Currency = currency,
                            MarketCapitalization = marktkapitalisierung ?? nettovermoegen,
                            CntEmployees = mitarbeiter,
                            Sector = sektor,
                            Industry = branche,
                            AvgTradingVolume = avgTradingVolume,
                            Url = driver.Url,
                            Name = strName,
                            Symbol = symbolAndType.symbol
                        };
                        result.Add(metaData);
                    });
                }
                catch (Exception ex)
                {
                    var errMsg = "\n- currentUrl: " + driver.Url + "\n";
                    errMsg += "- symbol: " + symbolAndType.symbol + "\n";
                    errMsg += "- exception: " + (ex.InnerException?.ToString() ?? ex.ToString()) + "\n";
                    _logger.LogWarning(ex, $"Url valid, but failed with an exception {errMsg}. In most cases the stock doesnt exists in Yahoo.com.");
                    //driver.SaveScreenshot($"{DateTime.UtcNow:yyyy-MM-dd}_{tradingAsset?.Id}.png");
                }
            }
        }
        catch (Exception ex)
        {
            throw new MarketDataException("Failed to fetch meta data", ex);
        }
        finally
        {
            _webDriverPoolManager.CloseAndDisposeDriver(driver);
        }
        return result;
    }

    //public async Task<List<YahooCourse>> GetCourses(string symbol, DateTime startDate, DateTime? endDate = null)
    //{
    //    IWebDriver driver = null;
    //    WebDriverWait wait = null;
    //    var coursesToAdd = new List<YahooCourse>();
    //    var policy = Policy.Handle<Exception>()
    //        .WaitAndRetry(3, retryAttempt => TimeSpan.FromMilliseconds(500));

    //    if (endDate == null || endDate?.Date >= DateTime.UtcNow.Date)
    //    {
    //        endDate = DateTime.UtcNow.AddDays(-1).Date;
    //    }
    //    if (startDate > endDate)
    //    {
    //        throw new TradeXException("Startdate after Endate");
    //    }

    //    try
    //    {
    //        driver = _webDriverPoolManager.GetDriver(false, _seleniumOptions.UseRemoteDriver, _seleniumOptions.Url);
    //        wait = _webDriverPoolManager.CreateWebDriverWait(driver, TimeSpan.FromSeconds(3));

    //        await policy.Execute(async () =>
    //        {
    //            driver.WaitForPageReady(true);
    //            driver.WaitForPageReady(true);
    //            driver.WaitForPageReady(true);
    //            driver.WaitForPageReady(true);

    //            driver.Navigate().GoToUrl("https://finance.yahoo.com");
    //            driver.WaitForPageReady(true);
    //            try
    //            {
    //                var privacyButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[@name='agree']")));
    //                privacyButton.Click();
    //                driver.SwitchTo().ParentFrame();
    //                driver.WaitForPageReady();
    //            }
    //            catch { }

    //            var cookieContainer = new CookieContainer();
    //            foreach (var cook in driver.Manage().Cookies.AllCookies)
    //            {
    //                var cookie = new System.Net.Cookie
    //                {
    //                    Name = cook.Name,
    //                    Value = cook.Value,
    //                    Domain = cook.Domain
    //                };
    //                cookieContainer.Add(cookie);
    //            }
    //            using var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
    //            using var httpClient = new HttpClient(handler);

    //            var requestUrl = $"https://query1.finance.yahoo.com/v7/finance/download/{HttpUtility.UrlEncode(symbol)}?period1={startDate.ToUnixTimeSeconds()}&period2={endDate.Value.AddDays(1).ToUnixTimeSeconds()}&interval=1d&events=history&includeAdjustedClose=true";
    //            var response = await httpClient.GetAsync(requestUrl);
    //            response.EnsureSuccessStatusCode();

    //            using var reader = new StreamReader(response.Content.ReadAsStream());
    //            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
    //            var records = csv.GetRecords<YahooCourseStringValues>()?.OrderBy(e => e.Date)?.ToList();

    //            if ((endDate - startDate).Value.Days > 100 && records.Count <= 20)
    //            {
    //                _logger.LogWarning($"Only {records.Count} records for {symbol}");
    //            }
    //            foreach (var record in records)
    //            {
    //                var okDate = DateTime.TryParse(record.Date, out var date);
    //                var okClosePrice = double.TryParse(record.Close, CultureInfo.InvariantCulture, out var closePrice);
    //                var okOpenPrice = double.TryParse(record.Open, CultureInfo.InvariantCulture, out var openPrice);
    //                var okVolumen = long.TryParse(record.Volume, CultureInfo.InvariantCulture, out var volume);

    //                if (!okDate || !okClosePrice || !okOpenPrice)
    //                {
    //                    _logger.LogError($"Parsing error: date={record.Date}, close={record.Close}, volume={record.Volume} of {symbol}");
    //                    continue;
    //                }
    //                if (date.Date > endDate.Value.Date)
    //                {
    //                    continue;
    //                }
    //                if (coursesToAdd.Any(e => e.Date == date.Date))
    //                {
    //                    var errMsg = $"Yahoo Bug: " +
    //                        $"Course for {symbol} for {record.Date:yyyy-MM-dd} already added!";
    //                    _logger.LogError(errMsg);
    //                }
    //                else
    //                {
    //                    coursesToAdd.Add(new YahooCourse
    //                    {
    //                        Date = date,
    //                        Open = openPrice,
    //                        Close = closePrice,
    //                        Volume = okVolumen ? volume : null
    //                    });
    //                }
    //            }
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        var exMessage = ex?.InnerException?.Message ?? "";
    //        if (!exMessage.Contains("(Not Found)"))
    //        {
    //            var errMsg = "\n- symbol: " + symbol + "\n";
    //            errMsg += "- exception: " + (ex.InnerException?.ToString() ?? ex.ToString()) + "\n";
    //            _logger.LogError(ex, errMsg);
    //        }
    //        _logger.LogWarning(exMessage);
    //    }
    //    finally
    //    {
    //        _webDriverPoolManager.ReleaseDriver(driver);
    //    }
    //    Console.WriteLine($"XXXXXXX Courses added for {symbol}");
    //    return coursesToAdd;
    //}

    public async Task<List<AssetCourse>> GetCourses(string symbol, DateTime startDate, DateTime? endDate = null)
    {
        var coursesToAdd = new List<AssetCourse>();
        var policy = Policy.Handle<Exception>()
            .WaitAndRetry(3, retryAttempt => TimeSpan.FromMilliseconds(500));

        if (endDate == null || endDate?.Date >= DateTime.UtcNow.Date)
        {
            endDate = DateTime.UtcNow.AddDays(-1).Date;
        }
        if (startDate > endDate)
        {
            throw new MarketDataException("Startdate after Endate");
        }
        try
        {
            await policy.Execute(async () =>
            {
                var cookieContainer = _yahooCookieManager.GetCookie();
                using var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                using var httpClient = new HttpClient(handler);

                var requestUrl = $"https://query1.finance.yahoo.com/v7/finance/download/{HttpUtility.UrlEncode(symbol)}?period1={startDate.ToUnixTimeSeconds()}&period2={endDate.Value.AddDays(1).ToUnixTimeSeconds()}&interval=1d&events=history&includeAdjustedClose=true";
                var response = await httpClient.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    cookieContainer = _yahooCookieManager.RenewCookie(cookieContainer);
                    throw new MarketDataException($"Cookie not valid anymore"); //retry
                }
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation($"No courses for {symbol} found");
                    return null;
                }
                response.EnsureSuccessStatusCode();

                using var reader = new StreamReader(response.Content.ReadAsStream());
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                var records = csv.GetRecords<YahooCourseStringValues>()?.OrderBy(e => e.Date)?.ToList();

                if ((endDate - startDate).Value.Days > 100 && records.Count <= 20)
                {
                    _logger.LogWarning($"Only {records.Count} records for {symbol}");
                }
                foreach (var record in records)
                {
                    var okDate = DateTime.TryParse(record.Date, out var date);
                    var okClosePrice = double.TryParse(record.Close, CultureInfo.InvariantCulture, out var closePrice);
                    var okOpenPrice = double.TryParse(record.Open, CultureInfo.InvariantCulture, out var openPrice);
                    var okVolumen = long.TryParse(record.Volume, CultureInfo.InvariantCulture, out var volume);

                    if (!okDate || !okClosePrice || !okOpenPrice)
                    {
                        _logger.LogDebug($"Parsing error: date={record.Date}, close={record.Close}, volume={record.Volume} of {symbol}");
                        continue;
                    }
                    if (date.Date > endDate.Value.Date)
                    {
                        continue;
                    }
                    if (coursesToAdd.Any(e => e.Date == date.Date))
                    {
                        var errMsg = $"Yahoo Bug: " +
                            $"Course for {symbol} for {record.Date:yyyy-MM-dd} already added!";
                        _logger.LogError(errMsg);
                    }
                    else
                    {
                        coursesToAdd.Add(new AssetCourse
                        {
                            Date = date,
                            Open = openPrice,
                            Close = closePrice,
                            Volume = okVolumen ? volume : null
                        });
                    }
                }

                return Task.CompletedTask;
            });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "XXXXXXXXXXXXXXXXX Unhadled ");
            if (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Handle unauthorized exception here
                Console.WriteLine("XXXXXXXXXXXXXXXXX Unauthorized access. Please check credentials or permissions.");
            }
            else
            {
                // Handle other types of HTTP request exceptions
                Console.WriteLine($"XXXXXXXXXXXXXXXXX An error occurred: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            var exMessage = ex?.InnerException?.Message ?? "";
            //if (!exMessage.Contains("(Not Found)"))
            //{
            var errMsg = $"symbol: {symbol}, exception: " + (ex.InnerException?.ToString() ?? ex.ToString()) + "\n";
            _logger.LogError(ex, errMsg);
            //}
            //_logger.LogWarning(exMessage);
        }
        return coursesToAdd;
    }
}
