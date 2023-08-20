namespace MarketData.Application.Interfaces;

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

public interface IWebDriverPoolManager
{
    public IWebDriver GetDriver(bool enableJs, bool useRemoteDriver, Uri remoteUrl);
    public WebDriverWait CreateWebDriverWait(IWebDriver driver, TimeSpan timeout);
    public void RecycleDriver(IWebDriver driver);
    public void CloseAndDisposeDriver(IWebDriver driver);
    public void Dispose();
}
