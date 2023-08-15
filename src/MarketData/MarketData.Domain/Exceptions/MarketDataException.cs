namespace MarketData.Domain.Exceptions;

using System;

public class MarketDataException : Exception
{
    public MarketDataException()
    {
    }

    public MarketDataException(string message)
        : base(message)
    {
    }

    public MarketDataException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
