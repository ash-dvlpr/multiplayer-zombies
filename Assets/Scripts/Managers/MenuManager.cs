using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum MenuID : int {
    None     = -1,
    Main     =  0,
    Settings =  1,
    Lobby    =  2, // TODO: Lobby Menu for Multiplayer
    Pause    =  3,
}

public static class MenuManager {
    public static bool Initialised { get; private set; } = false;
    public static MenuID CurrentMenu { get; private set; } = MenuID.None;

    private static GameObject mainCanvas;
    private static Dictionary<MenuID, Menu> menuChache;


    // ===================== Custom Code =====================
    public static void Init() {
        if (!Initialised) { 
            menuChache = new Dictionary<MenuID, Menu>();
            mainCanvas = GameObject.Find("MainCanvas");
            GameObject.DontDestroyOnLoad(mainCanvas);

            // Cache all menu objects
            foreach (Transform child in mainCanvas.transform) {
                var menu = child.GetComponent<Menu>();
                if (!menu || MenuID.None == menu.MenuKey) continue;

                menuChache[menu.MenuKey] = menu;
            }

            Initialised = true;

            InputManager.InGame_OnPause += PauseHandler;
        }
    }

    public static void Cleanup() {
        InputManager.InGame_OnPause -= PauseHandler;
        Initialised = false;
    }


    // ======================== Events ========================
    private static void PauseHandler(InputAction.CallbackContext ctx) {
        if (MenuID.None == CurrentMenu) OpenMenu(MenuID.Pause);
    }


    // ================== Outside Facing API ==================
    public static void ResetSelectedUIObject() {
        //var selected = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Opens up a menu based on it's identifier and closes the previouslly open menu.
    /// After opening a menu it caches it.
    /// </summary>
    /// <param name="menu"><see cref="MenuID">MenuID</see> of the menu that you want to open.</param>
    public static void OpenMenu(MenuID menu) {
        if (!Initialised) Init();
        var previous = CurrentMenu;
        CurrentMenu = menu;

        try {
            // TODO: Menu transitions

            // Show new Menu
            if (menuChache.TryGetValue(menu, out var newMenu)) {
                newMenu.OpenMenu();
            }

            // Hide old Menu
            if (menuChache.TryGetValue(previous, out var currentMenu)) {
                currentMenu.CloseMenu();
            }
        }
        catch (KeyNotFoundException ke) {
            Debug.LogError($"MenuManger.OpenMenu({menu}, {previous})");
            Debug.LogException(ke, mainCanvas);
        }
    }

    /// <summary>
    /// Closes up the currently open menu.
    /// </summary>
    public static void CloseMenu() {
        if (!Initialised) Init();

        try {
            if (menuChache.TryGetValue(CurrentMenu, out var menu)) {
                menu.CloseMenu();
            }
        }
        catch (KeyNotFoundException ke) {
            Debug.LogError($"MenuManger.CloseMenu(): {CurrentMenu} not found.");
            Debug.LogException(ke, mainCanvas);
        }
    }
}
