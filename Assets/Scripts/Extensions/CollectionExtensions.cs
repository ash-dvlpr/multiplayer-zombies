using System;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;

// All methdods will only be accesible if this namespace is imported
namespace CustomExtensions.Collections {

    // Extensions for Lists
    public static class ListExtensions {
        public static T GetRandom<T>(this List<T> list) {
            if (list.Count == 0) return default(T);
            else return list[Random.Range(0, list.Count)];
        }
    }
}
