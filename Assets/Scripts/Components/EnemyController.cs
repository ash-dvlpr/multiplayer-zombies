using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;

[RequireComponent(typeof(NavMeshAgent), typeof(Health))]
public class EnemyController : NetworkBehaviour {
    // ==================== Configuration ====================
    [Header("Combat")]
    [SerializeField] float attackRate = 2f;
    [SerializeField] int attackDamage = 10;

    [Header("Death")]
    [SerializeField] float timeBeforeCorpseRemoval = 3f;

    // ====================== References =====================
    public Health EnemyHealth { get => health; }

    NavMeshAgent agent;
    Animator animator;
    Health health;

    // ====================== Variables ======================
    [SyncVar] Transform _target;
    bool _attacking = false;


    // ====================== Unity Code ======================
    public override void OnStartServer() {
        base.OnStartServer();

        // Register enemy on the enemy spawner
        EnemySpawner.Instance?.Enemies.Add(this);

        // Start searching for targets
        StartCoroutine(SearchTargets());
    }

    public override void OnStopServer() { 
        base.OnStopServer(); 

        // Deregister enemy on the enemy spawner
        EnemySpawner.Instance?.Enemies.Remove(this);

        // Cleaup
        StopAllCoroutines();
    }

    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        health = GetComponent<Health>();
    }

    void OnEnable() {
        health.OnDeath += OnDeath;
    }

    void OnDisable() {
        health.OnDeath -= OnDeath;
    }
    void Update() {
        if (health.IsAlive && _target) {
            var target = _attacking ? this.transform.position : _target.position;
            if (null != target) {
                agent.destination = target;
            }

            bool moving = agent.velocity.magnitude > 1;
            animator.SetBool(AnimatorID.isRunning, moving);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (health.IsAlive && !_attacking && other.CompareTag("Player")) {
            StartCoroutine(AttackDelay());
            animator.SetTrigger(AnimatorID.triggerAttack);
            Attack(other.transform);
        }
    }

    void OnDeath() {
        if (IsServer) StopAllCoroutines();

        animator.SetBool(AnimatorID.isRunning, false);
        agent.destination = this.transform.position;
        agent.isStopped = true;

        if (base.IsServer) StartCoroutine(DelayCorposeRemoval());
    }

    [ServerRpc(RequireOwnership = false)]
    void Attack(Transform other) {
        var hitHp = other.GetComponent<Health>();
        hitHp?.Damage(attackDamage);
    }

    // ===================== Custom Code =====================
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
    IEnumerator SearchTargets() {
        while (true) { 
            ChangeTarget();
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator AttackDelay() {
        _attacking = true;
        yield return new WaitForSeconds(attackRate);
        _attacking = false;
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
        var players = EnemySpawner.Instance?.Players.Where(
            p => p.PlayerHealth.IsAlive
        ).ToList();


        GameObject nearestPlayer = null;
        if (players.Count > 0) { 
            var shortestDistance = Mathf.Infinity;

            foreach (var p in players) {
                var vDistance = p.transform.position - transform.position;
                // We are just comparing distances, we don't need precision, so we can save up on a Mathf.Sqrt()
                var distance = vDistance.sqrMagnitude;
            
                if (shortestDistance > distance) {
                    nearestPlayer = p.gameObject;
                    shortestDistance = distance;
                }
            }
        }

        // If no targets found, stay in place
        _target = nearestPlayer != null ? nearestPlayer.transform : this.transform;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeTarget(Transform newTarget) {
        _target = newTarget;
    }
}
