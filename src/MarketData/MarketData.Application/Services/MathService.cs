namespace MarketData.Application.Services;
using System.Linq;
using MarketData.Application.Interfaces;
using MarketData.Application.Repositories;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearRegression;
using MathNet.Numerics.Statistics;
#pragma warning disable CA1822 // Mark members as static

public class MathService : IMathService, IScopedService
{
    public bool MatchedDirection(double? predictedDp, double? realDp)
    {
        if (predictedDp == null || realDp == null) return false;
        return predictedDp > 0 && realDp > 0 || predictedDp < 0 && realDp < 0 || predictedDp == 0 && realDp == 0;
    }

    public float? CalculateError(float? realPriceDp, float? predictedPriceDp)
    {
        if (realPriceDp == null || predictedPriceDp == null)
        {
            return null;
        }

        return Math.Abs(realPriceDp.Value - predictedPriceDp.Value);
    }

    public decimal? CalculateDeltaPercentage(long? buyCourse, long? sellCourse)
    {
        return CalculateDeltaPercentageInternal(buyCourse, sellCourse);
    }
    public decimal CalculateDeltaPercentage(decimal buyCourse, decimal sellCourse)
    {
        return (decimal)CalculateDeltaPercentageInternal(buyCourse, sellCourse);
    }

    public decimal? CalculateDeltaPercentage(decimal? buyCourse, decimal? sellCourse)
    {
        return CalculateDeltaPercentageInternal(buyCourse, sellCourse);
    }

    //public double CalculateDeltaPercentage(double buyCourse, double sellCourse)
    //{
    //    return (double)CalculateDeltaPercentageInternal((decimal)buyCourse, (decimal)sellCourse);
    //}
    public float? CalculateDeltaPercentage(double? buyCourse, double? sellCourse)
    {
        return (float?)CalculateDeltaPercentageInternal((decimal?)buyCourse, (decimal?)sellCourse);
    }

    private decimal? CalculateDeltaPercentageInternal(decimal? buyCourse, decimal? sellCourse)
    {
        if (buyCourse == null || sellCourse == null)
        {
            return null;
        }

        if (buyCourse == 0)
        {
            return 0;
        }
        if (buyCourse < 0 && sellCourse > 0)
        {
            return -((sellCourse - buyCourse) / buyCourse * 100);
        }
        return (sellCourse - buyCourse) / buyCourse * 100;
    }

    public decimal? CalculateDifference(decimal? calculated, decimal? real)
    {
        if (calculated == null || real == null)
        {
            return null;
        }

        if (calculated > real)
        {
            return calculated - real;
        }

        return real - calculated;
    }

    public float CalculatePercentage(int currentValue, int maxValue)
    {
        return (float)currentValue / maxValue * 100f;
    }

    public (Vector<double> vectorM, Vector<double> vectorR, double rMedian) MultipleRegression(Matrix matrixX, Vector<double> vectorY, DirectRegressionMethod method = DirectRegressionMethod.NormalEquations)
    {
        var vectorM = MathNet.Numerics.LinearRegression.MultipleRegression
                            .DirectMethod(matrixX, vectorY, method);

        var reconstructY = matrixX * vectorM;
        var vectorR = reconstructY - vectorY;
        var rMedian = vectorR.Select(e => Math.Abs(e)).Median();

        return (vectorM, vectorR, rMedian);
    }

    //public (List<double> coeffs03, List<double> coeffs0305, List<double> coeffs0507, List<double> coeffs0709, List<double> coeffs0915, List<double> coeffs15) CategorizeCoeffs(List<double> coeffs)
    //{
    //    var coeffsWithouutK0 = coeffs.Skip(1).ToList();
    //    //var coeffs_00 = coeffs?.Where(e => Math.Abs(e) == 0)?.ToList();
    //    var coeffs03 = coeffsWithouutK0?.Where(e => Math.Abs(e) < 0.3)?.ToList();
    //    var coeffs0305 = coeffsWithouutK0?.Where(e => Math.Abs(e) >= 0.3 && Math.Abs(e) < 0.5)?.ToList();
    //    var coeffs0507 = coeffsWithouutK0?.Where(e => Math.Abs(e) >= 0.5 && Math.Abs(e) < 0.7)?.ToList();
    //    var coeffs0709 = coeffsWithouutK0?.Where(e => Math.Abs(e) >= 0.7 && Math.Abs(e) < 0.9)?.ToList();
    //    var coeffs0915 = coeffsWithouutK0?.Where(e => Math.Abs(e) >= 0.9 && Math.Abs(e) < 1.5)?.ToList();
    //    var coeffs15 = coeffsWithouutK0?.Where(e => Math.Abs(e) >= 1.5)?.ToList();

    //    return (coeffs03, coeffs0305, coeffs0507, coeffs0709, coeffs0915, coeffs15);
    //}

    //public (List<double> coeffs, List<int> srcStockIds) FilterCoeffs(List<double> coeffs, List<int> srcStockIds, double minCoeff, double maxCoeff)
    //{
    //    var resultCoeffs = new List<double>();
    //    var resultSrcStockIds = new List<int>();

    //    for (var coeffIdx = 1; coeffIdx < coeffs.Count; coeffIdx++)
    //    {
    //        var coeff = coeffs[coeffIdx];
    //        if (Math.Abs(coeff) >= minCoeff && Math.Abs(coeff) <= maxCoeff)
    //        {
    //            var srcStockId = srcStockIds[coeffIdx - 1];
    //            resultCoeffs.Add(coeff);
    //            resultSrcStockIds.Add(srcStockId);
    //        }
    //    }
    //    return (resultCoeffs, resultSrcStockIds);
    //}

    /// <summary>
    /// Method to calculate Mean Absolute Error (MAE) using LINQ
    /// </summary>
    /// <param name="predictedPrices"></param>
    /// <param name="realPrices"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public float? CalculateMAE(IEnumerable<float> predictedPrices, IEnumerable<float> realPrices)
    {
        if (predictedPrices.Count() != realPrices.Count())
            throw new ArgumentException("The number of predicted prices must match the number of real prices.");

        var n = predictedPrices.Count();
        if (n == 0) return null;
        var mae = (float)predictedPrices.Zip(realPrices, (predicted, real) => Math.Abs(predicted - real)).Sum() / n;
        if (float.IsNaN(mae))
        {
            return null; // Division by zero or negative value in logarithm
        }
        return mae;
    }

    public float? CalculateMaeRatio(IEnumerable<float> predictedPrices, IEnumerable<float> realPrices)
    {
        if (predictedPrices.Count() != realPrices.Count())
            throw new ArgumentException("The number of predicted prices must match the number of real prices.");

        var n = predictedPrices.Count();
        if (n == 0) return null;
        var mae = (float)predictedPrices.Zip(realPrices, (predicted, real) => Math.Abs(predicted - real)).Sum() / n;
        if (float.IsNaN(mae))
        {
            return null; // Division by zero or negative value in logarithm
        }
        var avg = predictedPrices.Average();
        if (avg == 0 || avg < 0.1 || mae < 0.1) return null;
        var maeRatio = mae / avg;
        return mae;

        //        (Meta_All_MAE / Meta_All_Avg_AbsPredictedDp) AS difference, Algo_Id as algoId
        //FROM TradeX.AlgoSimulations
        //where Meta_All_Avg_AbsPredictedDp > 0.1 and Meta_All_MAE > 0.1  and `Date` IN('2023-08-04')
        //-- order by difference asc;

    }

    /// <summary>
    /// Method to calculate Mean Absolute Percentage Error (MAPE) using LINQ
    /// </summary>
    /// <param name="predictedPrices"></param>
    /// <param name="realPrices"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public float? CalculateMAPE(IEnumerable<float> predictedPrices, IEnumerable<float> realPrices)
    {
        if (predictedPrices.Count() != realPrices.Count())
            throw new ArgumentException("The number of predicted prices must match the number of real prices.");

        var n = predictedPrices.Count();
        if (n == 0) return null;
        var mape = (float)predictedPrices.Zip(realPrices, (predicted, real) =>
        {
            if (real != 0)
            {
                return Math.Abs((predicted - real) / real);
            }
            else
            {
                return 0; // Handle division by zero
            }
        }).Sum() / n * 100;
        if (float.IsNaN(mape))
        {
            return null; // Division by zero or negative value in logarithm
        }
        return mape;
    }

    /// <summary>
    /// Method to calculate Mean Squared Error (MSE) using LINQ
    /// </summary>
    /// <param name="predictedPrices"></param>
    /// <param name="realPrices"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public float? CalculateMSE(IEnumerable<float> predictedPrices, IEnumerable<float> realPrices)
    {
        if (predictedPrices.Count() != realPrices.Count())
            throw new ArgumentException("The number of predicted prices must match the number of real prices.");

        var n = predictedPrices.Count();
        if (n == 0) return null;
        var mse = (float)predictedPrices.Zip(realPrices, (predicted, real) => Math.Pow(predicted - real, 2)).Sum() / n;
        if (float.IsNaN(mse))
        {
            return null; // Division by zero or negative value in logarithm
        }
        return mse;
    }

    /// <summary>
    /// RMSLE (Root Mean Squared Logarithmic Error)
    /// </summary>
    /// <param name="actualValues"></param>
    /// <param name="predictedValues"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public float? CalculateRMSLE(IEnumerable<float> actualValues, IEnumerable<float> predictedValues)
    {
        int n = actualValues.Count();

        if (n != predictedValues.Count() || n == 0)
        {
            return null;
        }

        float meanSquaredLogError = actualValues.Zip(predictedValues, (actual, predicted) =>
            (float)Math.Pow(Math.Log(1 + actual) - Math.Log(1 + predicted), 2)).Average();

        if (float.IsNaN(meanSquaredLogError) || float.IsInfinity(meanSquaredLogError))
        {
            return null; // Division by zero or negative value in logarithm
        }

        return (float)Math.Sqrt(meanSquaredLogError);
    }

    /// <summary>
    /// MAE% (Mean Absolute Error Percentage):
    /// </summary>
    /// <param name="actualValues"></param>
    /// <param name="predictedValues"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public float? CalculateMAEPercentage(IEnumerable<float> actualValues, IEnumerable<float> predictedValues)
    {
        int n = actualValues.Count();

        if (n != predictedValues.Count() || n == 0)
        {
            return null;
        }

        float sumAbsolutePercentageError = actualValues.Zip(predictedValues, (actual, predicted) =>
            Math.Abs((actual - predicted) / actual)).Sum();

        if (float.IsNaN(sumAbsolutePercentageError))
        {
            return null; // Division by zero or negative value in logarithm
        }

        return sumAbsolutePercentageError / n * 100f;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actualValues"></param>
    /// <param name="predictedValues"></param>
    /// <returns></returns>
    public float? CalculateRMAE(IEnumerable<float> actualValues, IEnumerable<float> predictedValues)
    {
        float? mae = CalculateMAE(actualValues, predictedValues);

        if (mae == null)
        {
            return null; // Division by zero
        }

        return (float)Math.Sqrt((float)mae);
    }

    public float? CalculateRAE(IEnumerable<float> actualValues, IEnumerable<float> predictedValues)
    {
        int n = actualValues.Count();

        if (n != predictedValues.Count() || n == 0)
        {
            return null;
        }

        float sumAbsoluteError = actualValues.Zip(predictedValues, (actual, predicted) =>
            Math.Abs(actual - predicted)).Sum();

        float meanActualValue = actualValues.Average();
        float sumBaselineError = actualValues.Sum(val => Math.Abs(val - meanActualValue));

        if (sumBaselineError == 0)
        {
            return null; // Division by zero
        }

        return sumAbsoluteError / sumBaselineError;
    }

    public float? CalculateAbsR2(IEnumerable<float> actualValues, IEnumerable<float> predictedValues)
    {
        int n = actualValues.Count();

        if (n != predictedValues.Count() || n == 0)
        {
            return null;
        }

        float meanActualValue = actualValues.Average();

        float sumOfSquaresTotal = actualValues.Sum(val => (float)Math.Pow(val - meanActualValue, 2));
        float sumOfSquaresResidual = actualValues.Zip(predictedValues, (actual, predicted) =>
            (float)Math.Pow(actual - predicted, 2)).Sum();

        if (sumOfSquaresTotal == 0)
        {
            return null; // Division by zero
        }

        return Math.Abs(1.0f - sumOfSquaresResidual / sumOfSquaresTotal);
    }

    public float? CalculateMedAE(IEnumerable<float> actualValues, IEnumerable<float> predictedValues)
    {
        int n = actualValues.Count();

        if (n != predictedValues.Count() || n == 0)
        {
            return null;
        }

        float[] absoluteErrors = actualValues.Zip(predictedValues, (actual, predicted) =>
            Math.Abs(actual - predicted)).ToArray();

        Array.Sort(absoluteErrors);
        int medianIndex = n / 2;

        if (n % 2 == 0)
        {
            return (absoluteErrors[medianIndex] + absoluteErrors[medianIndex - 1]) / 2;
        }
        else
        {
            return absoluteErrors[medianIndex];
        }
    }

    public (bool? directionMatched, float? deltaPercentageDirectionBuyWin) CalculateDirectionMatched(float? calculatedDeltaPercentage, float? realDeltaPercentage)
    {
        bool? directionMatched;
        float? deltaPercentageDirectionBuy = null;
        if (calculatedDeltaPercentage == null || realDeltaPercentage == null)
        {
            return (null, null);
        }
        if (Math.Round((float)calculatedDeltaPercentage.Value * 10000) / 10000f == 0 && Math.Round((float)realDeltaPercentage.Value * 10000) / 10000f == 0)
        {
            directionMatched = false;
        }
        else
        {
            if (calculatedDeltaPercentage == realDeltaPercentage ||
                calculatedDeltaPercentage < 0 && realDeltaPercentage < 0 ||
                calculatedDeltaPercentage > 0 && realDeltaPercentage > 0)
            {
                directionMatched = true;
            }
            else
            {
                directionMatched = false;
            }
        }
        if (realDeltaPercentage != null)
        {
            if (calculatedDeltaPercentage >= 0)
            {
                deltaPercentageDirectionBuy = realDeltaPercentage.Value;
            }
        }
        return (directionMatched, deltaPercentageDirectionBuy);
    }


    public double CalculateNextPrice(double lastPrice, float priceDp)
    {
        //var calculatedPrice = lastPrice * (1 + priceDp / 100);
        var calculatedPrice = lastPrice + lastPrice * priceDp / 100;
        return calculatedPrice;
    }

    public (List<double> coeffs03, List<double> coeffs0305, List<double> coeffs0507, List<double> coeffs0709, List<double> coeffs0915, List<double> coeffs15) CategorizeCoeffs(List<double> coeffs)
    {
        var coeffsWithouutK0 = coeffs.Skip(1).ToList();
        //var coeffs_00 = coeffs?.Where(e => Math.Abs(e) == 0)?.ToList();
        var coeffs03 = coeffsWithouutK0?.Where(e => Math.Abs(e) < 0.3)?.ToList();
        var coeffs0305 = coeffsWithouutK0?.Where(e => Math.Abs(e) >= 0.3 && Math.Abs(e) < 0.5)?.ToList();
        var coeffs0507 = coeffsWithouutK0?.Where(e => Math.Abs(e) >= 0.5 && Math.Abs(e) < 0.7)?.ToList();
        var coeffs0709 = coeffsWithouutK0?.Where(e => Math.Abs(e) >= 0.7 && Math.Abs(e) < 0.9)?.ToList();
        var coeffs0915 = coeffsWithouutK0?.Where(e => Math.Abs(e) >= 0.9 && Math.Abs(e) < 1.5)?.ToList();
        var coeffs15 = coeffsWithouutK0?.Where(e => Math.Abs(e) >= 1.5)?.ToList();

        return (coeffs03, coeffs0305, coeffs0507, coeffs0709, coeffs0915, coeffs15);
    }

    public (List<double> coeffs, List<int> srcStockIds) FilterCoeffs(List<double> coeffs, List<int> srcStockIds, double minCoeff, double maxCoeff)
    {
        var resultCoeffs = new List<double>();
        var resultSrcStockIds = new List<int>();

        for (var coeffIdx = 1; coeffIdx < coeffs.Count; coeffIdx++)
        {
            var coeff = coeffs[coeffIdx];
            if (Math.Abs(coeff) >= minCoeff && Math.Abs(coeff) <= maxCoeff)
            {
                var srcStockId = srcStockIds[coeffIdx - 1];
                resultCoeffs.Add(coeff);
                resultSrcStockIds.Add(srcStockId);
            }
        }
        return (resultCoeffs, resultSrcStockIds);
    }
}
#pragma warning restore CA1822 // Mark members as static
