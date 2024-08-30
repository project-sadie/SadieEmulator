using System.Collections.Concurrent;

namespace Sadie.Shared.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetOrInsert<TKey, TValue>(
        this ConcurrentDictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TValue> valueFactory)
        where TKey : notnull
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            return value;
        }
        
        value = valueFactory();
        dictionary[key] = value;
        
        return value;
    }
}