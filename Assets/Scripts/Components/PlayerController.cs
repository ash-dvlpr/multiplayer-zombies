using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;
using FishNet.Component.Animating;

[RequireComponent(typeof(PlayerMovement), typeof(Health), typeof(Ammo))]
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
    [SyncVar] bool _canControl = false;
    [SyncVar] bool _canShoot = false;

    public bool CanMove { get => base.IsOwner && GameManager.IsPlaying; }
    public bool CanControl {
        get => _canControl && !GameManager.ClientInMenu;
        [Server]
        set => _canControl = value;
    }
    public bool CanShoot { 
        get => CanControl && ammo.HasAmmo && _canShoot; 
        private set => _canShoot = value;
    }

    public Health PlayerHealth { get => health; }
    public Ammo PlayerAmmo { get => ammo; }
    Vector3 _spawnPosition;

    // ====================== References =====================
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] Weapon weapon;
    PlayerMovement playerMovement;

    PlayerUI playerUI;
    ResourceBar hpBar;
    ResourceBar ammoBar;
    Health health;
    Ammo ammo;

    // ======================= NetCode ========================
    public override void OnStartServer() {
        base.OnStartServer();

        CanControl = CanShoot = true;
        _spawnPosition = this.transform.position;

        // Register player on the enemy spawner
        EnemySpawner.Instance?.Players.Add(this);

        health.OnDeath += EnemySpawner.Instance.OnPlayerDied;
    }

    public override void OnStopServer() {
        base.OnStartServer();

        // Deregister player on the enemy spawner
        EnemySpawner.Instance?.Players.Remove(this);

        health.OnDeath -= EnemySpawner.Instance.OnPlayerDied;
    }


    public override void OnStartClient() {
        base.OnStartClient();

        if (base.IsOwner) {
            hpBar.SwapTrackedResource(health);
            ammoBar.SwapTrackedResource(ammo);
        }
        else {
            virtualCamera.enabled = false;
        }

        GameManager.Instance.OnRoundStart += RoundStart;
        GameManager.Instance.OnRoundEnd += RoundEnd;
    }

    public override void OnStopClient() {
        base.OnStopClient();

        GameManager.Instance.OnRoundStart -= RoundStart;
        GameManager.Instance.OnRoundEnd += RoundEnd;
    }
    // ====================== Unity Code ======================
    void Awake() {
        if (!virtualCamera) virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        if (!weapon) weapon = GetComponentInChildren<Weapon>();

        playerMovement = GetComponent<PlayerMovement>();
        health = GetComponent<Health>();
        ammo = GetComponent<Ammo>();

        playerUI = (PlayerUI) MenuManager.Get(MenuID.PlayerUI);
        hpBar = playerUI.HPBar;
        ammoBar = playerUI.AmmoBar;
    }

    void OnEnable() {
        health.OnDeath += OnDeath;
        InputManager.InGame_OnShoot += ShootHandler;
    }

    void OnDisable() {
        health.OnDeath -= OnDeath;
        InputManager.InGame_OnShoot -= ShootHandler;
    }

    // ===================== Custom Code =====================
    [Server]
    public void Restore() {
        CanShoot = true;
        CanControl = true;
        health.ResetValues();
        ammo.ResetValues();
        playerMovement.RequestTeleport(_spawnPosition);
    }
    void RoundStart() {
        Debug.Log("Round Started");
        if (base.IsServer) {
            CanControl = CanShoot = true;
        }

        // TODO: Round start message
    }
    void RoundEnd() {
        Debug.Log("Round Ended");
        if (base.IsServer) {
            CanControl = false;
            CanShoot = false;
        }
    }
    void OnDeath() {
        Debug.Log("Player died");
        CanControl = false;
        // TODO: trigger death animation

        //Destroy(this, timeBeforeCorpseRemoval);
    }

    [Client]
    void ShootHandler(InputAction.CallbackContext ctx) {
        if (CanMove && CanShoot) {
            StartCoroutine(ApplyShootingDelay());
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

    [ServerRpc]
    void Shoot(Vector3 cameraPosition, Vector3 direction) {
        if (ammo.HasAmmo) {
            ammo.Consume(1);

            if (Physics.Raycast(cameraPosition, direction, out var hit, maxShotDistance, damageableLayers)) {
                var hitHp = hit.transform.GetComponent<Health>();
                hitHp?.Damage(shotDamage);
            }
        }
    }
}
