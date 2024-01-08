using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Managing.Scened;
using UnityEngine.SceneManagement;
using System;

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
    private void LoadScene(string sceneName, bool @override = false) {
        if (!IsServer && !@override) return;

        SceneLoadData sld = new SceneLoadData(sceneName);
        //sld.Options.AutomaticallyUnload = true;
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }

    private void UnloadScene(string sceneName, bool @override = false) {
        if (!IsServer && !@override) return;

        if (@override) {
            try { 
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            } catch (ArgumentException e) {
                Debug.LogWarning($"NetSceneManager.UnloadScene({sceneName}, {@override}): {e}");
            }
        }
        else {
            SceneUnloadData sud = new SceneUnloadData(sceneName);
            InstanceFinder.SceneManager.UnloadGlobalScenes(sud);
        }
    }

    //? Exposed methods for the Lobby Code
    public void LoadGraveyardScene(bool @override = false) {
        LoadScene(GameManager.SCENE_ID_GRAVEYARD, @override);
    }
    public void UnloadGraveyardScene(bool @override = false) {
        UnloadScene(GameManager.SCENE_ID_GRAVEYARD, @override);
    }
}
