using System;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

// All methdods will only be accesible if this namespace is imported
namespace CustomExtensions.Collections {

    // Extensions for Arrays
    public static class AarrayExtensions {
        public static bool IsNullOrEmpty<T>(this T[] arr) {
            return (arr == null || arr.Length == 0);
        }
        public static bool NotNullOrEmpty<T>(this T[] arr) {
            return !arr.IsNullOrEmpty();
        }
    }

    // Extensions for Lists
    public static class ListExtensions {
        public static T GetRandom<T>(this List<T> list) {
            if (list.Count == 0) return default(T);
            else return list[Random.Range(0, list.Count)];
        }
    }

    // Extensions for IEnumerables
    public static class IEnumerableExtensions { 
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action) {
            foreach (T item in enumeration) {
                action(item);
            }
        }
    }
}
