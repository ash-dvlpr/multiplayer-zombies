using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed  = 12f;

    [Header("Camera")]
    [SerializeField] private float lookSpeed;

    //? Variables
    private bool canMove, isRunning;
    private Vector3 moveDirection = Vector3.zero;

    //? References
    private PlayerController player;


    // =======================================================
    void Start() {
        player = GetComponent<PlayerController>();
    }

    void Update() {
        if (canMove) {
            //? Move
            var vForward = transform.TransformDirection(Vector3.forward);
            var vRight = transform.TransformDirection(Vector3.right);

            // Get speed from inputs
            var speed = isRunning ? runSpeed : walkSpeed;
            float mvSpeedX = speed * Input.GetAxis("Horizontal");
            float mvSpeedY = speed * Input.GetAxis("Vertical");

            var mvDirY = moveDirection.y;
            moveDirection = (vForward * mvSpeedX) + (vRight * mvSpeedY);

            //? Jump
            // TODO: moveDirection.y = jumpPressed && canJump ? jumpForce : mvDirY;
            // TODO: if (!grounded) moveDirection.y -= gravity * Time.deltaTime;

            // TODO: Update camera
            // TODO: https://www.youtube.com/watch?v=qQLvcS9FxnY
        }
    }
}
