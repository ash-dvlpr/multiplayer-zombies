using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement), typeof(Health))]
public class PlayerController : MonoBehaviour {
    // ==================== Configuration ====================
    [Header("Shooting")]
    [SerializeField] Animator weaponHandleAnimator;
    [SerializeField] LayerMask damageableLayers;
    [SerializeField] float maxShotDistance;
    [SerializeField] int shotDamage = 20;

    //[Header("Death")]
    //[SerializeField] float timeBeforeCorpseRemoval = 4f;

    // ====================== References =====================
    [SerializeField] Weapon weapon;
    CharacterController characterController;
    PlayerMovement playerMovement;
    Health health;

    // ====================== Unity Code ======================
    void Awake() {
        characterController = GetComponent<CharacterController>();    
        playerMovement = GetComponent<PlayerMovement>();
        health = GetComponent<Health>();
    }

    void Start() {
        // Hide Mouse Cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnEnable() {
        InputManager.InGame_OnShoot += ShootHandler;
        health.OnDeath += OnDeath;
    }

    void OnDisable() {
        InputManager.InGame_OnShoot -= ShootHandler;
        health.OnDeath -= OnDeath;
    }

    // ===================== Custom Code =====================
    void RoundStart() {
        playerMovement.CanMove = true;
    }
    void RoundEnd() {
        playerMovement.CanMove = false;
    }

    void ShootHandler(InputAction.CallbackContext ctx) {
        var camPos = Camera.main.transform.position;
        var camDir = Camera.main.transform.forward;
        weaponHandleAnimator.SetTrigger(AnimatorID.triggerAttack);
        Shoot(camPos, camDir);
    }    

    void Shoot(Vector3 cameraPosition, Vector3 direction) {
        if (Physics.Raycast(cameraPosition, direction, out var hit, maxShotDistance, damageableLayers)) {
            var hitHp = hit.transform.GetComponent<Health>();
            hitHp?.Damage(shotDamage);
        }
    }

    void OnDeath() {
        //Destroy(this, timeBeforeCorpseRemoval);
        Debug.Log("Player died");
    }
}
