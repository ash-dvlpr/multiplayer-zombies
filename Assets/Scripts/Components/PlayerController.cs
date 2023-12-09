using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;
using FishNet.Component.Animating;

[RequireComponent(typeof(PlayerMovement), typeof(Health))]
public class PlayerController : NetworkBehaviour {
    // ==================== Configuration ====================
    [Header("Shooting")]
    [SerializeField] NetworkAnimator weaponAnimator;
    [SerializeField] LayerMask damageableLayers;
    [SerializeField] float maxShotDistance;
    [SerializeField] float shootingRate = 0.1f;
    [SerializeField] int shotDamage = 20;

    //[Header("Death")]
    //[SerializeField] float timeBeforeCorpseRemoval = 4f;

    // ====================== Variables ======================
    [SyncVar] bool _canShoot = false;
    bool _canMove = false;
    public bool CanMove { 
        get => IsOwner && GameManager.IsPlaying && _canMove && !GameManager.IsPaused; 
        private set => _canMove = value; 
    }
    public bool CanShoot { get => _canShoot; private set => _canShoot = value; }

    // ====================== References =====================
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] Weapon weapon;
    //PlayerMovement playerMovement;

    PlayerUI playerUI;
    ResourceBar hpBar;
    Health health;

    // ======================= NetCode ========================
    public override void OnStartClient() { 
        base.OnStartClient();

        if (!IsOwner) virtualCamera.enabled = false;

        _canMove = _canShoot = GameManager.IsPlaying;
    }

    // ====================== Unity Code ======================
    void Awake() {
        if (!virtualCamera) virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        if (!weapon) weapon = GetComponentInChildren<Weapon>();

        //playerMovement = GetComponent<PlayerMovement>();
        health = GetComponent<Health>();
        playerUI = (PlayerUI) MenuManager.Get(MenuID.PlayerUI);
        hpBar = playerUI.Bar;
        hpBar.SwapTrackedResource(health);
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
        CanShoot = true;
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
        if (CanMove && CanShoot) {
            StartCoroutine(ApplyShootingDelay());
        }
    }

    [ServerRpc(RunLocally = true)]
    void Shoot(Vector3 cameraPosition, Vector3 direction, NetworkConnection sender = null) {
        // TODO: consume ammo
        

        if (Physics.Raycast(cameraPosition, direction, out var hit, maxShotDistance, damageableLayers)) {
            var hitHp = hit.transform.GetComponent<Health>();
            hitHp?.Damage(shotDamage);
        }
    }

    IEnumerator ApplyShootingDelay() {
        if (CanShoot) {
            CanShoot = false;
            
            var camPos = Camera.main.transform.position;
            var camDir = Camera.main.transform.forward;

            weaponAnimator.SetTrigger(AnimatorID.triggerAttack);
            Shoot(camPos, camDir);

            yield return new WaitForSeconds(shootingRate);
        }

        CanShoot = true;
    }
}
