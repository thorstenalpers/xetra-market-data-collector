namespace MarketData.Infratructure.Services;

using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Polly;
using SeleniumExtras.WaitHelpers;

public static class WebDriverExtensions
{
    public static void WaitForPageReady(this IWebDriver driver, bool sleep = false, int timeoutSec = 3)
    {
        var policy = Policy.Handle<Exception>()
            .WaitAndRetry(10, retryAttempt => TimeSpan.FromMilliseconds(500));

        policy.Execute(() =>
        {
            var js = (IJavaScriptExecutor)driver;
            var wait = new WebDriverWait(driver, new TimeSpan(0, 0, timeoutSec));
            wait.Until(wd => js.ExecuteScript("return document.readyState")?.ToString() == "complete");
            if (sleep)
            {
                Thread.Sleep(500);
            }
            if (sleep)
            {
                Thread.Sleep(500);
            }
        });
    }

    public static string GetText(this IWebDriver driver, By by, WebDriverWait wait)
    {
        try
        {
            var text = wait.Until(ExpectedConditions.ElementExists(by)).Text;
            if (text == "N/A" || text == "-")
            {
                return null;
            }
            return text;
        }
        catch
        {
            return null;
        }
    }

    public static bool RetryClick(this IWebDriver driver, By by, WebDriverWait wait, By waitBy, bool sleep = false, int nTimes = 3)
    {
        for (var i = 0; i < nTimes; i++)
        {
            try
            {
                driver.WaitForPageReady(false);
                wait.Until(ExpectedConditions.ElementToBeClickable(by)).Click();
                driver.WaitForPageReady(sleep);

                if (waitBy != null)
                {
                    wait.Until(ExpectedConditions.ElementExists(waitBy)).Click();
                    driver.WaitForPageReady(sleep);
                }
                return true;
            }
            catch
            {
                if (i + 1 < nTimes)
                {
                    driver.Navigate().Refresh();
                }
                else
                {
                    throw;
                }

                driver.WaitForPageReady(sleep);
                if (sleep)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
        }
        return false;
    }
}
