using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FishNet.Object;

[RequireComponent(typeof(PlayerMovement), typeof(Health))]
public class PlayerController : NetworkBehaviour {
    // ==================== Configuration ====================
    [Header("Shooting")]
    [SerializeField] Animator weaponHandleAnimator;
    [SerializeField] LayerMask damageableLayers;
    [SerializeField] float maxShotDistance;
    [SerializeField] int shotDamage = 20;

    //[Header("Death")]
    //[SerializeField] float timeBeforeCorpseRemoval = 4f;

    // ====================== Variables ======================
    private bool _canMove = false;
    public bool CanMove { get => _canMove && IsOwner; private set => _canMove = value; }

    // ====================== References =====================
    [SerializeField] Weapon weapon;
    //PlayerMovement playerMovement;
    Health health;

    // ======================= NetCode ========================
    public override void OnStartClient() { 

    }

    // ====================== Unity Code ======================
    void Awake() {
        //playerMovement = GetComponent<PlayerMovement>();
        health = GetComponent<Health>();
    }

    void OnEnable() {
        health.OnDeath += OnDeath;
        GameManager.Instance.OnRoundStart += RoundStart;
        GameManager.Instance.OnRoundEnd += RoundEnd;
        InputManager.InGame_OnShoot += ShootHandler;
    }

    void OnDisable() {
        health.OnDeath -= OnDeath;
        GameManager.Instance.OnRoundStart -= RoundStart;
        GameManager.Instance.OnRoundEnd += RoundEnd;
        InputManager.InGame_OnShoot -= ShootHandler;
    }

    // ===================== Custom Code =====================
    void RoundStart() {
        CanMove = true;
    }
    void RoundEnd() {
        CanMove = false;
    }
    void OnDeath() {
        Debug.Log("Player died");
        CanMove = false;
        //Destroy(this, timeBeforeCorpseRemoval);
    }

    void ShootHandler(InputAction.CallbackContext ctx) {
        if (CanMove) {
            var camPos = Camera.main.transform.position;
            var camDir = Camera.main.transform.forward;

            weaponHandleAnimator.SetTrigger(AnimatorID.triggerAttack);
            Shoot(camPos, camDir);
        }
    }

    void Shoot(Vector3 cameraPosition, Vector3 direction) {
        if (Physics.Raycast(cameraPosition, direction, out var hit, maxShotDistance, damageableLayers)) {
            var hitHp = hit.transform.GetComponent<Health>();
            hitHp?.Damage(shotDamage);
        }
    }
}
