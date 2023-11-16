using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

[RequireComponent(typeof(PlayerController), typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed  = 12f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpGravity = 1f;
    [SerializeField] private float fallGravity = 3f;

    [Header("Camera")]
    [SerializeField] private float lookSpeed;
    [SerializeField] private float lookMaxAngle = 80f;

    //? Variables
    private bool canMove, isRunning;
    private Vector3 moveDirection = Vector3.zero;
    private float cameraPitch = 0;

    private bool jumpPressed = false;
    private float xAxis = 0f, yAxis = 0f, mouseX = 0f, mouseY = 0f;

    //? References
    [Header("GameObject Dependencies")]
    [SerializeField] private GameObject playerCamera;
    PlayerController player;
    Rigidbody rb;

    // =======================================================
    void Start() {
        player = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        canMove = true;
    }

    void Update() {
        UpdateInputs();
        if (canMove) {
            //? Move
            var vForward = transform.TransformDirection(Vector3.forward);
            var vRight = transform.TransformDirection(Vector3.right);

            // Get speed from inputs
            var speed = isRunning ? runSpeed : walkSpeed;
            float mvSpeedX = speed * xAxis; 
            float mvSpeedY = speed * yAxis;

            Debug.Log($"Speed: {mvSpeedX} X, {mvSpeedY} Y");

            var mvDirY = moveDirection.y;
            moveDirection = (vForward * mvSpeedX) + (vRight * mvSpeedY);

            //? Jump
            // TODO: check canJump
            moveDirection.y = jumpPressed ? jumpForce : mvDirY;
            // TODO: if (!grounded)
            moveDirection.y -= (jumpPressed ? jumpGravity : fallGravity) * Time.deltaTime;
            
            // Update velocity
            rb.velocity = moveDirection;

            // TODO: Update camera
            cameraPitch -= mouseY * lookSpeed;
            cameraPitch = Mathf.Clamp(cameraPitch, -lookMaxAngle, lookMaxAngle);
            playerCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
            transform.rotation *= Quaternion.Euler(0, mouseX * lookSpeed, 0);

            // TODO: https://www.youtube.com/watch?v=qQLvcS9FxnY
        }
    }

    void UpdateInputs() {
        jumpPressed = Input.GetButton("Jump");

        xAxis = Input.GetAxis("Vertical");
        yAxis = Input.GetAxis("Horizontal");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }
}
