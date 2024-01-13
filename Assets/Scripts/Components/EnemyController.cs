using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Component.Animating;

[RequireComponent(typeof(NavMeshAgent), typeof(Health))]
public class EnemyController : NetworkBehaviour {

    // ==================== Configuration ====================
    [Header("Sounds")]
    [SerializeField] AudioClip movingSound;
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip randomSound;
    [SerializeField] AudioClip deathSound;

    [Header("Combat")]
    [SerializeField] float attackRate = 2f;
    [SerializeField] int attackDamage = 10;

    [Header("Death")]
    [SerializeField] float timeBeforeCorpseRemoval = 3f;

    // ====================== References =====================
    public Health EnemyHealth { get => health; }

    NavMeshAgent agent;
    Animator animator;
    NetworkAnimator netAnimator;
    Health health;
    AudioSource audioSource;
    Collider _collider;

    // ====================== Variables ======================
    [SyncVar] Transform _target;
    //NavMeshPath path;
    bool _attacking = false;


    // ====================== Unity Code ======================
    public override void OnStartNetwork() {
        base.OnStartNetwork();

        if (!base.IsServer) {
            agent.enabled = false;
        }

        // Register events
        health.OnDeath += OnDeath;
    }
    public override void OnStopNetwork() {
        base.OnStopNetwork();

        // Deregister events
        health.OnDeath -= OnDeath;
    }
    public override void OnStartServer() {
        base.OnStartServer();

        _collider = GetComponent<Collider>();
        animator.SetBool(AnimatorID.isAlive, true);

        // Register enemy on the enemy spawner
        NetGameManager.Instance?.Enemies.Add(this);

        // Start searching for targets
        StartCoroutine(SearchTargets());
        PlayMovingSound();
    }

    public override void OnStartClient() {
        base.OnStartClient();

        audioSource = this.gameObject.AddComponent<AudioSource>();
        
        // Register enemy on the enemy spawner
        AudioManager.Instance?.entitySources.Add(audioSource);
    }
    public override void OnStopClient() {
        base.OnStopClient();

        // Register enemy on the enemy spawner
        AudioManager.Instance?.entitySources.Remove(audioSource);
    }

    public override void OnStopServer() {
        base.OnStopServer();

        // Deregister enemy on the enemy spawner
        NetGameManager.Instance?.Enemies.Remove(this);

        // Cleaup
        StopAllCoroutines();
    }

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        netAnimator = GetComponentInChildren<NetworkAnimator>();
        health = GetComponent<Health>();
    }

    void Update() {
        if (
            // Is alive and has a target
            base.IsServer && health.IsAlive && null != _target
            // and singleplayer player has not paused the game
            && GameManager.IsPlaying
        ) {
            var target = _attacking ? this.transform.position : _target.position;
            if (null != target) {
                agent.destination = target;
            }

            bool moving = agent.velocity.magnitude > 0.1f;
            animator.SetBool(AnimatorID.isRunning, moving);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (health.IsAlive && !_attacking && other.CompareTag("Player")) {
            PlayAttackSound();
            if (base.IsServer) {
                StartCoroutine(AttackDelay());
                netAnimator.SetTrigger(AnimatorID.triggerAttack);
                Attack(other.transform);
            }
        }
    }


    // ===================== Custom Code =====================
    void OnDeath() {
        StopAllCoroutines();
        
        if (base.IsServer) { 
            agent.destination = this.transform.position;
            agent.isStopped = true;

            animator.SetBool(AnimatorID.isRunning, false);
            animator.SetBool(AnimatorID.isAlive, false);

            NetGameManager.Instance.AliveEnemies--;
        }
            
        _collider.enabled = false;

        if (base.IsClient) {
            AudioManager.PlayClipOn(deathSound, audioSource);
        }

        if (base.IsServer) StartCoroutine(DelayCorposeRemoval());
    }

    [Server]
    void Attack(Transform other) {
        var hitHp = other.GetComponent<Health>();
        hitHp?.Damage(attackDamage);
    }

    [Server]
    IEnumerator SearchTargets() {
        while (true) {
            ChangeTarget();
            yield return new WaitForSeconds(NetGameManager.Instance?.TimeBetweenPathRecalculations ?? 1);
        }
    }

    IEnumerator AttackDelay() {
        _attacking = true;
        yield return new WaitForSeconds(attackRate);
        _attacking = false;
        PlayMovingSound();
    }

    [Server]
    IEnumerator DelayCorposeRemoval() {
        yield return new WaitForSeconds(timeBeforeCorpseRemoval);
        base.Despawn(this.gameObject);
    }

    [Server]
    private void ChangeTarget() {
        // Get players from EnemySpawner's cached player list
        // And filter for alive players
        var players = from p in NetGameManager.Instance?.Players
                      where p.TryGet<Health>(out var health) && health.IsAlive
                      select p;

        GameObject nearestPlayer = null;
        GameObject nearestPathablePlayer = null;
        if (players.Any()) {
            var shortestDistance = Mathf.Infinity;
            var shortestPathableDistance = Mathf.Infinity;

            // Calculate closest pathabe player and the mathematically closest player
            foreach (var p in players) {
                var vDistance = p.transform.position - transform.position;
                // We are just comparing distances, we don't need precision, so we can save up on a Mathf.Sqrt()
                var distance = vDistance.sqrMagnitude;

                // Check if that player is reachable
                var path = new NavMeshPath();
                agent.CalculatePath(p.transform.position, path);

                // Store the true shortest distance in case there are no players with a full path
                if (shortestDistance > distance) {
                    nearestPlayer = p.gameObject;
                    shortestDistance = distance;
                }

                // Skip unreachable targets
                if (NavMeshPathStatus.PathInvalid == path.status) continue;

                if (shortestPathableDistance > distance) {
                    nearestPathablePlayer = p.gameObject;
                    shortestPathableDistance = distance;
                }
            }

            // If there were no pathable players, use the mathematically closest player
            _target = nearestPathablePlayer == null ? nearestPlayer.transform : nearestPathablePlayer.transform;
        }
        // If there are no alive players
        else {
            _target = this.transform;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeTarget(Transform newTarget) {
        _target = newTarget;
    }

    /* TODO: config
     * - Serializable Transform for attack position + radious
     * - Draw Attack range gizmo
     * 
     * TODO: attack rework
     * - If round has started, start coroutine to replace the target objective every couple seconds
     * - Then, on Update, chech distance to target, and if target on attac range, trigger an attack.
     */

    //void Attack() {
    //    if (!_attacking) {
    //        Physics.OverlapSphere();
    //        // if player in range, continue attacking
    //        AttackDelay();
    //        // TODO: damage
    //    }
    //}


    // ======================= Sounds ========================
    [ObserversRpc(BufferLast = true)]
    public void PlayMovingSound() {
        if (base.IsClient && movingSound != null)
            AudioManager.PlayClipOn(movingSound, audioSource);
    }

    [ObserversRpc]
    public void PlayAttackSound() {
        if (base.IsClient && attackSound != null)
            AudioManager.PlayClipOn(attackSound, audioSource);
    }

    [ObserversRpc]
    public void PlayRandomSound() {
        if (base.IsClient && randomSound != null)
            AudioManager.PlayClipOn(randomSound, audioSource);
    }
}
