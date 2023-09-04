using System.Collections.Generic;

public static class DictionaryExtensions
{
    public static void AddOrIncreaseValue<TKey>(
        this Dictionary<TKey, int> dictionary, TKey key, int value)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] += value;
        }
        else
        {
            dictionary.Add(key, value);
        }
    }
}
