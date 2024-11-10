namespace Sadie.Shared.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<IEnumerable<TSource>?> Batch<TSource>(this IEnumerable<TSource> source, int size)
    {
        TSource[]? bucket = null;
        var count = 0;

        foreach (var item in source)
        {
            bucket ??= new TSource[size];
            bucket[count++] = item;
            
            if (count != size)
            {
                continue;
            }

            yield return bucket;

            bucket = null;
            count = 0;
        }

        if (bucket != null && count > 0)
        {
            yield return bucket.Take(count).ToArray();
        }
    }
    
    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        return source.PickRandom(1).Single();
    }

    private static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
    {
        return source.Shuffle().Take(count);
    }

    private static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(_ => Guid.NewGuid());
    }
}