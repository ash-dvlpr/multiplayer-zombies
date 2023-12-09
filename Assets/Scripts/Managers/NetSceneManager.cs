using FishNet;
using FishNet.Managing.Scened;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetSceneManager : MonoBehaviour {
    public static NetSceneManager Instance { get; private set; }
    void Awake() {
        // Mantain a single Instance
        if (Instance != null && Instance != this) DestroyImmediate(this);
        else {
            DontDestroyOnLoad(this);
            Instance = this;
        }
    }

    // ====================== Variables ======================
    private bool IsServer { get => InstanceFinder.IsServer; }


    // =================== Scene Management ===================
    private void LoadScene(string sceneName) {
        if (!IsServer) return;

        SceneLoadData sld = new SceneLoadData(sceneName);
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }

    private void UnloadScene(string sceneName) {
        if (!IsServer) return;

        SceneUnloadData sud = new SceneUnloadData(sceneName);
        InstanceFinder.SceneManager.UnloadGlobalScenes(sud);
    }

    //? Exposed methods for the Lobby Code
    public void LoadCityScene() {
        LoadScene(GameManager.SCENE_ID_CITY);
    }
    public void UnloadCityScene() {
        UnloadScene(GameManager.SCENE_ID_CITY);
    }
}
