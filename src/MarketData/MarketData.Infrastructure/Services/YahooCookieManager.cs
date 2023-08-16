namespace MarketData.Infrastructure.Services;

using OpenQA.Selenium;
using Microsoft.Extensions.Options;
using SeleniumExtras.WaitHelpers;
using System.Net;
using Microsoft.Extensions.Logging;
using MarketData.Domain.Repositories;
using MarketData.Domain.Exceptions;
using MarketData.Infrastructure.Options;

public interface IYahooCookieManager
{
    void InvalidateCookie(CookieContainer cookieContainer);
    CookieContainer RenewCookie(CookieContainer cookieContainer);
    CookieContainer GetCookie();
    bool IsValid(CookieContainer cookieContainer);
}
public class YahooCookieManager : IYahooCookieManager, ISingletonService
{
    private readonly object _lock = new object();
    private readonly IWebDriverPoolManager _webDriverPoolManager;
    private readonly SeleniumOptions _seleniumOptions;
    private CookieContainer _cookieContainer;
    private readonly ILogger<YahooCookieManager> _logger;

    public YahooCookieManager(IOptions<SeleniumOptions> options, ILogger<YahooCookieManager> logger, IWebDriverPoolManager webDriverPoolManager)
    {
        _seleniumOptions = options.Value;
        _webDriverPoolManager = webDriverPoolManager;
        _logger = logger;
    }

    public CookieContainer GetCookie()
    {
        lock (_lock)
        {
            if (_cookieContainer == null)
            {
                _cookieContainer = RenewCookie(null);
            }
            return _cookieContainer;
        }
    }

    public bool IsValid(CookieContainer cookieContainer)
    {
        lock (_lock)
        {
            if (cookieContainer == null || cookieContainer != _cookieContainer)
            {
                return false;
            }
            return true;
        }
    }

    public void InvalidateCookie(CookieContainer cookieContainer)
    {
        lock (_lock)
        {
            _cookieContainer = null;
        }
    }

    public CookieContainer RenewCookie(CookieContainer cookieContainer)
    {
        lock (_lock)
        {
            // parallel http unauthorized exceptions, so renew already triggered
            if (_cookieContainer != null && cookieContainer != _cookieContainer)
                return _cookieContainer;

            IWebDriver webDriver = null;

            try
            {
                webDriver = _webDriverPoolManager.GetDriver(false, _seleniumOptions.UseRemoteDriver, _seleniumOptions.Url);
                var wait = _webDriverPoolManager.CreateWebDriverWait(webDriver, TimeSpan.FromSeconds(3));

                webDriver.WaitForPageReady(true);
                webDriver.WaitForPageReady(true);
                webDriver.WaitForPageReady(true);
                webDriver.WaitForPageReady(true);

                webDriver.Navigate().GoToUrl("https://finance.yahoo.com");
                webDriver.WaitForPageReady(true);
                try
                {
                    var privacyButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[@name='agree']")));
                    privacyButton.Click();
                    webDriver.SwitchTo().ParentFrame();
                    webDriver.WaitForPageReady();
                }
                catch { }

                var newCookieContainer = new CookieContainer();
                foreach (var cookie in webDriver.Manage().Cookies.AllCookies)
                {
                    var newCookie = new System.Net.Cookie
                    {
                        Name = cookie.Name,
                        Value = cookie.Value,
                        Domain = cookie.Domain
                    };
                    newCookieContainer.Add(newCookie);
                }
                _cookieContainer = newCookieContainer;
                _logger.LogInformation("Yahoo.com cookie renewed");
                return _cookieContainer;
            }
            catch (Exception ex)
            {
                var exMessage = ex?.InnerException?.Message ?? "";
                _logger.LogWarning(exMessage);
            }
            finally
            {
                _webDriverPoolManager.CloseAndDisposeDriver(webDriver);
            }
        }
        throw new MarketDataException("CookieContainer was not created");
    }
}
