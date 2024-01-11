using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITypeCache {
    public void Register<T>(T @object);
    public bool TryGet<T>(out T @object);
}