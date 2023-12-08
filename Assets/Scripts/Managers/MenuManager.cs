using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        menuChache = new Dictionary<MenuID, Menu>();
        mainCanvas = GameObject.Find("MainCanvas");

        // Cache all menu objects
        foreach (Transform child in mainCanvas.transform) {
            var menu = child.GetComponent<Menu>();
            if (!menu || MenuID.None == menu.MenuKey) continue;

            menuChache[menu.MenuKey] = menu;
        }

        Initialised = true;
    }

    private static void Cleanup() {
        Initialised = false;
        CurrentMenu = MenuID.None;
    }

    public static void ResetSelectedUIObject() {
        //var selected = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public static void OpenMenu(MenuID menu, MenuID current = MenuID.None) {
        if (!Initialised) Init();

        if (MenuID.None == current) current = CurrentMenu;
        CurrentMenu = menu;

        try {
            // TODO: Menu transitions

            // Show new Menu
            if (menuChache.TryGetValue(menu, out var newMenu)) {
                newMenu.OpenMenu();
            }

            // Hide old Menu
            if (menuChache.TryGetValue(current, out var currentMenu)) {
                currentMenu.CloseMenu();
            }
        }
        catch (KeyNotFoundException ke) {
            Debug.LogError($"MenuManger.OpenMenu({menu}, {current}): ");
            Debug.LogException(ke, mainCanvas);
        }
    }
}
