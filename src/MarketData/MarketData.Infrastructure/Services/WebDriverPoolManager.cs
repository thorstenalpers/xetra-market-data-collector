namespace MarketData.Infrastructure.Services;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using Microsoft.Extensions.Options;
using MarketData.Domain.Repositories;
using MarketData.Domain.Exceptions;
using MarketData.Infrastructure.Options;

public interface IWebDriverPoolManager
{
    public IWebDriver GetDriver(bool enableJs, bool useRemoteDriver, Uri remoteUrl);
    public WebDriverWait CreateWebDriverWait(IWebDriver driver, TimeSpan timeout);
    public void RecycleDriver(IWebDriver driver);
    public void CloseAndDisposeDriver(IWebDriver driver);
    public void Dispose();
}
public class WebDriverPoolManager : IWebDriverPoolManager, IDisposable, ISingletonService
{
    private readonly object _lock = new object();
    private readonly List<IWebDriver> _usedDrivers = new List<IWebDriver>();
    private readonly Queue<IWebDriver> _availableDrivers = new Queue<IWebDriver>();
    private readonly int _maxDriverInstances;

    public WebDriverPoolManager(IOptions<SeleniumOptions> options)
    {
        _maxDriverInstances = options.Value?.MaxParallelism ?? 0;
    }

    public IWebDriver GetDriver(bool enableJs, bool useRemoteDriver, Uri remoteUrl)
    {
        lock (_lock)
        {
            if (_availableDrivers.Any())
            {
                var driver = _availableDrivers.Dequeue();
                _usedDrivers.Add(driver);
                return driver;
            }
            if (_usedDrivers.Count >= _maxDriverInstances)
            {
                throw new MarketDataException($"No WebDriver available");
            }

            var newDriver = CreateWebDriver(enableJs, useRemoteDriver, remoteUrl);
            _usedDrivers.Add(newDriver);
            return newDriver;
        }
    }

    public void RecycleDriver(IWebDriver driver)
    {
        if (driver == null) return;
        lock (_lock)
        {
            _usedDrivers.Remove(driver);
            _availableDrivers.Enqueue(driver);
        }
    }

    public void CloseAndDisposeDriver(IWebDriver driver)
    {
        if (driver == null) return;
        lock (_lock)
        {
            _usedDrivers.Remove(driver);
            driver.Close();
            driver.Quit();
            driver.Dispose();
        }
    }

    public WebDriverWait CreateWebDriverWait(IWebDriver driver, TimeSpan timeout)
    {
        return new WebDriverWait(driver, timeout);
    }

    private static IWebDriver CreateWebDriver(bool enableJs, bool useRemoteDriver, Uri remoteUrl)
    {
        IWebDriver driver;
        var firefoxOptions = new FirefoxOptions();
        firefoxOptions.SetLoggingPreference(LogType.Browser, LogLevel.Warning);
        firefoxOptions.SetPreference("javascript.enabled", enableJs);
        firefoxOptions.SetPreference("permissions.default.desktop-notification", 1);
        firefoxOptions.SetPreference("media.navigator.permission.disabled", true);
        firefoxOptions.SetPreference("intl.accept_languages", "de-DE");
        firefoxOptions.SetPreference("browser.cache.disk.enable", false);
        firefoxOptions.SetPreference("browser.cache.memory.enable", false);
        firefoxOptions.SetPreference("browser.cache.offline.enable", false);
        firefoxOptions.SetPreference("network.http.use-cache", false);

        if (useRemoteDriver)
        {
            driver = new RemoteWebDriver(remoteUrl, firefoxOptions.ToCapabilities(), TimeSpan.FromMinutes(3));
        }
        else
        {
            driver = new FirefoxDriver(firefoxOptions);
        }
        if (enableJs)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        }
        return driver;
    }

    public void Dispose()
    {
        lock (_lock)
        {
            foreach (var driver in _usedDrivers.Concat(_availableDrivers))
            {
                driver?.Quit();
                driver?.Dispose();
            }
            _usedDrivers.Clear();
            _availableDrivers.Clear();
        }
        GC.SuppressFinalize(this);
    }
}
