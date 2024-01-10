using System;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;


namespace CustomExtensions.Collections {
    public static class ListExtensions {
        public static T GetRandom<T>(this List<T> list) {
            return list[Random.Range(0, list.Count)];
        }
    }
}
