﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Core.Extensions
{
    public static class GenericExtensions
    {
        public static bool IsNotNull(this object obj)
        {
            return obj is object;
        }

        public static bool IsNotNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection.IsNotNull() && collection.Count > 0;
        }

        public static bool IsFilled(this string str)
        {
            return !String.IsNullOrEmpty(str);
        }

        public static bool NotContains<T>(this ICollection<T> collection, T element)
        {
            return !collection.Contains(element);
        }

        public static bool TryAddWithRetries<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, TValue value, int retryCount)
        {
            int i = 0;
            while (i < retryCount)
            {
                if (dict.TryAdd(key, value))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool TryRemoveWithRetries<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key, int retryCount)
        {
            int i = 0;
            while (i < retryCount)
            {
                if (dict.TryRemove(key, out TValue _))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
