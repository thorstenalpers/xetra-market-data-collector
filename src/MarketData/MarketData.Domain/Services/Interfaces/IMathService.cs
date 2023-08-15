namespace MarketData.Domain.Services.Interfaces;
#pragma warning disable CA1822 // Mark members as static

public interface IMathService
{
    public bool MatchedDirection(double? predictedDp, double? realDp);
    public float? CalculateDeltaPercentage(double? buyCourse, double? sellCourse);
}
#pragma warning restore CA1822 // Mark members as static
