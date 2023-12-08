using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuID : int {
    NONE      = -1,
    MAIN      =  0,
    SETTINGS  =  1,
    PAUSE     =  2,
}

public static class MenuManager {
    public static bool Initialised { get; private set; }

    private static GameObject mainCanvas;
    private static Dictionary<MenuID, Menu> menus;


    // ===================== Custom Code =====================
    public static void Init() {
        menus = new Dictionary<MenuID, Menu>();
        mainCanvas = GameObject.Find("MainCanvas");

        // Cache all menu objects
        foreach (Transform child in mainCanvas.transform) {
            var menu = child.GetComponent<Menu>();
            if (!menu || MenuID.NONE == menu.MenuKey) continue;

            menus[menu.MenuKey] = menu;
        }

        Initialised = true;
    }

    private static void Cleanup() { 
        Initialised = false;
    }

    public static void OpenMenu(MenuID menu, MenuID current) {
        if (!Initialised) Init();


        try {
            // TODO: Menu transitions

            // Show new Menu
            if (menus.TryGetValue(menu, out var newMenu)) {
                newMenu.gameObject.SetActive(true);
            }

            // Hide old Menu
            if (menus.TryGetValue(current, out var currentMenu)) {
                currentMenu.gameObject.SetActive(false);
            }
        } catch (KeyNotFoundException ke) {
            Debug.LogError($"MenuManger.OpenMenu({menu}, {current}): ");
            Debug.LogException(ke, mainCanvas);
        }
    }
}
