using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Health))]
public class PlayerController : MonoBehaviour {
    // ==================== Configuration ====================
    [Header("Shooting")]
    [SerializeField] LayerMask damageableLayers;
    [SerializeField] float maxShotDistance;
    [SerializeField] int shotDamage = 20;

    // ====================== References =====================
    CharacterController _characterController;
    [SerializeField] Weapon _weapon;

    // ====================== Unity Code ======================
    void Awake() {
        _characterController = GetComponent<CharacterController>();    
    }

    void Start() {
        // Hide Mouse Cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable() {
        InputManager.InGame_OnShoot += ShootHandler;    
    }

    void OnDisable() {
        InputManager.InGame_OnShoot -= ShootHandler;    
    }

    // ===================== Custom Code =====================
    void ShootHandler(InputAction.CallbackContext ctx) {
        var camPos = Camera.main.transform.position;
        var camDir = Camera.main.transform.forward;
        Shoot(camPos, camDir);
    }

    void Shoot(Vector3 cameraPosition, Vector3 direction) {
        if (Physics.Raycast(cameraPosition, direction, out var hit, maxShotDistance, damageableLayers)) {
            var hitHp = hit.transform.GetComponent<Health>();
            hitHp?.Damage(shotDamage);
        }
    }
}
