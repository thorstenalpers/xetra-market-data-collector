namespace MarketData.Application.Extensions;
public static class ArrayExtensions
{
    public static string ToVectorString(this double[] array)
    {
        return "{" + string.Join(", ", new List<double>(array).Select(i => i.ToString("F")).ToArray()) + "}";
    }

    public static string ToMatrixString(this double[][] array)
    {
        var result = "";
        for (var t = 0; t < array.Length; t++)
        {
            var equation = "\t{";
            for (var i = 0; i < array[t].Length; i++)
            {
                equation += $"{array[t][i]:F}";
                if (i + 1 < array[t].Length)
                {
                    equation += ", ";
                }
            }
            result += equation + "}";
            if (t + 1 < array.Length)
            {
                result += "\n";
            }
        }
        return result;
    }


    /// <summary>
    /// Split a list into batches, so that all sub lists have a length lower or equal than batchSize
    /// </summary>
    public static List<List<T>> SplitIntoBatches<T>(this List<T> originalList, int batchSize)
    {
        var batches = new List<List<T>>();
        for (int i = 0; i < originalList.Count; i += batchSize)
        {
            var batch = originalList.GetRange(i, Math.Min(batchSize, originalList.Count - i));
            batches.Add(batch);
        }
        return batches;
    }

    /// <summary>
    /// Split a list into equal size chunks, so that the result list have a length of totlSize
    /// </summary>
    public static List<List<T>> SplitIntoEqualSizedChunks<T>(this List<T> source, int totalSize)
    {
        int batchSize = (int)Math.Ceiling(source.Count / (double)totalSize);

        return source
            .Select((item, index) => new { item, index })
            .GroupBy(pair => pair.index / batchSize)
            .Select(group => group.Select(pair => pair.item).ToList())
            .ToList();
    }
}
