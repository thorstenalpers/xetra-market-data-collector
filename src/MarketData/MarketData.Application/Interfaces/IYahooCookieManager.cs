namespace MarketData.Application.Interfaces;
using System.Net;

public interface IYahooCookieManager
{
    void InvalidateCookie(CookieContainer cookieContainer);
    CookieContainer RenewCookie(CookieContainer cookieContainer);
    CookieContainer GetCookie();
    bool IsValid(CookieContainer cookieContainer);
}
