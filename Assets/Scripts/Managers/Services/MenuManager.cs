using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum MenuID : int {
    None       = 0,
    Main       = 1,
    Settings   = 2,
    Lobby      = 3,
    Pause      = 4,
    PlayerUI   = 5,
    GameOverUI = 6,
    Crossfade  = 7,
}

public static class MenuManager {
    public static bool Initialised { get; private set; } = false;
    public static MenuID CurrentMenu { get; private set; } = MenuID.None;

    private static GameObject mainCanvas;
    private static Dictionary<MenuID, AMenu> menuChache;


    // ===================== Custom Code =====================
    public static void Init() {
        if (!Initialised) { 
            menuChache = new Dictionary<MenuID, AMenu>();
            mainCanvas = GameObject.Find("MainCanvas");
            //GameObject.DontDestroyOnLoad(mainCanvas);
            mainCanvas.SetActive(true);

            // Cache all menu objects
            foreach (Transform child in mainCanvas.transform) {
                var menu = child.GetComponent<AMenu>();
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
        if (MenuID.PlayerUI == CurrentMenu) OpenMenu(MenuID.Pause);
        else if (MenuID.Pause == CurrentMenu) OpenMenu(MenuID.PlayerUI);
    }


    // ================== Outside Facing API ==================
    public static AMenu Get(MenuID id) {
        menuChache.TryGetValue(id, out var menu);
        return menu;
    }

    public static void ResetSelectedUIObject() {
        //var selected = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Opens up a menu based on it's identifier and closes the previouslly open menu.
    /// After opening a menu it caches it.
    /// </summary>
    /// <param name="menu"><see cref="MenuID">MenuID</see> of the menu that you want to open.</param>
    /// <param name="ignoreOldMenu">If set to true, will just show the menu without closing other menus or setting it as the last open menu.</param>
    public static void OpenMenu(MenuID menu, bool ignoreOldMenu = false) {
        if (!Initialised) Init();
        var previous = CurrentMenu;
        if (!ignoreOldMenu) {
            CurrentMenu = menu;
        }

        try {
            // Hide old Menu
            if (!ignoreOldMenu && menuChache.TryGetValue(previous, out var currentMenu)) {
                currentMenu.CloseMenu();
            }

            // TODO: Menu transitions

            // Show new Menu
            if (menuChache.TryGetValue(menu, out var newMenu)) {
                newMenu.OpenMenu();
            }
        }
        catch (KeyNotFoundException ke) {
            Debug.LogError($"MenuManger.OpenMenu({menu}, {previous})");
            Debug.LogException(ke, mainCanvas);
        }
    }

    /// <summary>
    /// Closes up the currently <see cref="CurrentMenu">opened menu</see>.
    /// Will only alter the <see cref="CurrentMenu">cached menu</see> only if you don't specify a menuID.
    /// </summary>
    /// <param name="menuID"><see cref="MenuID">MenuID</see> of the menu that you want to close.</param>
    public static void CloseMenu(MenuID menuID = MenuID.None) {
        if (!Initialised) Init();
        var targetMenu = MenuID.None == menuID ? CurrentMenu : menuID;


        try {
            if (menuChache.TryGetValue(targetMenu, out var menu)) {
                if (MenuID.None == menuID) { 
                    CurrentMenu = MenuID.None;
                }
                menu.CloseMenu();
            }
        }
        catch (KeyNotFoundException ke) {
            Debug.LogError($"MenuManger.CloseMenu(): {CurrentMenu} not found.");
            Debug.LogException(ke, mainCanvas);
        }
    }

    // Utils
    public static void FadeOut(bool forced = false) {
        Debug.Log("Fading Out");
        var crossfade = (Crossfade) Get(MenuID.Crossfade);
        crossfade.TriggerFadeOut(forced);
    }
    public static void FadeIn(bool forced = false) { 
        Debug.Log("Fading In");
        var crossfade = (Crossfade) Get(MenuID.Crossfade);
        crossfade.TriggerFadeIn(forced);
    }
}
