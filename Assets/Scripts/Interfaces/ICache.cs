using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICache<K, V> where V : ICacheable<K> {
    public void Register(V @object);
    public bool TryGet(K key, out V @object);
}

public interface ICacheable<K> { 
    public K Key { get; }
}