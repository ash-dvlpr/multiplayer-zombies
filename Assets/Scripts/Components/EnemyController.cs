using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {
    // ====================== References =====================
    private NavMeshAgent agent;
    private Animator animator;
    private Transform _target;

    // ====================== Unity Code ======================
    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Start() {
        ChangeTarget();
    }

    void Update() {
        if (null != _target) {
            agent.destination = _target.position;
        }

        bool moving = agent.velocity.magnitude > 1;
        animator.SetBool(AnimatorID.isRunning, moving);
    }

    // ===================== Custom Code =====================
    private void ChangeTarget() {
        GameObject[] players;
        players = GameObject.FindGameObjectsWithTag("Player");


        GameObject nearestPlayer = null;

        float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject p in players) {
            var disatance = p.transform.position - position;
            float curDistance = disatance.sqrMagnitude;

            if (curDistance < distance) {
                nearestPlayer = p;
                distance = curDistance;
            }
        }

        ChangeTarget(nearestPlayer.transform);
    }

    public void ChangeTarget(Transform newTarget) {
        _target = newTarget;
    }
}
