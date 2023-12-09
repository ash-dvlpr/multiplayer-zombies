using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Scrapped concept
//
/*
public abstract class AService<T> : SingletonPersistent<T> where T : MonoBehaviour {
    protected abstract void Init();
    protected abstract void Cleanup();
}

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
    public static T instance { get; private set; }
    protected virtual void Awake() => instance = this as T;

    protected virtual void OnApplicationQuit() {
        instance = null;
        Destroy(gameObject);
    }
}

public abstract class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour {
    protected override void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        };
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }
}
*/