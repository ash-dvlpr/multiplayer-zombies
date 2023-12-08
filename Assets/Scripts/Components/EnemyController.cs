using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Health))]
public class EnemyController : MonoBehaviour {
    // ==================== Configuration ====================
    [Header("Combat")]
    [SerializeField] float attackRate = 2f;
    [SerializeField] int attackDamage = 10;

    [Header("Death")]
    [SerializeField] float timeBeforeCorpseRemoval = 4f;

    // ====================== References =====================
    NavMeshAgent agent;
    Animator animator;
    Health health;

    // ====================== Variables ======================
    Transform _target;
    bool _attacking = false;


    // ====================== Unity Code ======================
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

    void Start() {
        ChangeTarget();
    }

    void Update() {
        if (health.IsAlive) {
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

            var targetHealth = other.GetComponent<Health>();
            targetHealth.Damage(attackDamage);
        }
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
    IEnumerator AttackDelay() {
        _attacking = true;
        yield return new WaitForSeconds(attackRate);
        _attacking = false;
    }

    private void ChangeTarget() {
        // TODO: Get players from GameManager's cached player list
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject nearestPlayer = null;

        // TODO: Skip if no alive players
        var shortestDistance = Mathf.Infinity;

        foreach (var p in players) {
            var vDistance = p.transform.position - transform.position;
            // We are just comparing distances, we don't need precision, so we can save up on a Mathf.Sqrt()
            var distance = vDistance.sqrMagnitude;
            
            if (shortestDistance > distance) {
                nearestPlayer = p;
                shortestDistance = distance;
            }
        }

        if (nearestPlayer != null) {
            ChangeTarget(nearestPlayer.transform);
        }
    }

    public void ChangeTarget(Transform newTarget) {
        _target = newTarget;
    }

    void OnDeath() {
        animator.SetBool(AnimatorID.isRunning, false);
        agent.destination = this.transform.position;
        agent.isStopped = true;

        Destroy(this.gameObject, timeBeforeCorpseRemoval);
    }
}
