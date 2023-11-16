using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

[RequireComponent(typeof(PlayerController), typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed  = 12f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpGravity = 1f;
    [SerializeField] private float fallGravity = 3f;
    [SerializeField] private float groundCheckRange = 2.2f;

    [Header("Camera")]
    [SerializeField] private float lookSpeed;
    [SerializeField] private float lookMaxAngle = 80f;

    //? Variables
    private bool canMove = true, isGrounded;
    private Vector3 moveDirection = Vector3.zero;
    private float cameraPitch = 0;

    private bool jumpPressed, isRunning;
    private float xAxis = 0f, yAxis = 0f, mouseX = 0f, mouseY = 0f;

    //? References
    [Header("Other Configuration")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private LayerMask groundLayer;
    PlayerController player;
    Rigidbody rb;

    // =======================================================
    void Start() {
        player = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        canMove = true;
    }

    void OnDrawGizmos() {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckRange);
    }

    void Update() {
        UpdateInputs();
        if(canMove) {
            HandleCameraMovement();
            FixMovement();
        }
    }

    void FixedUpdate() {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckRange, groundLayer);

        if(canMove) {
            HandleMovement();
        }
    }

    // =======================================================
    void UpdateInputs() {
        jumpPressed = Input.GetButton("Jump");

        xAxis = Input.GetAxis("Vertical");
        yAxis = Input.GetAxis("Horizontal");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }

    void HandleCameraMovement() {
        // Update camera
        cameraPitch -= mouseY * lookSpeed;
        cameraPitch = Mathf.Clamp(cameraPitch, -lookMaxAngle, lookMaxAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        transform.rotation *= Quaternion.Euler(0, mouseX * lookSpeed, 0);
    }

    void FixMovement() {
        // Limit player velocity
        var curVel = rb.velocity; curVel.y = 0;

        var speed = isRunning ? runSpeed : walkSpeed;
        if (curVel.magnitude > speed) {
            var correctedVel = curVel.normalized * speed;
            correctedVel.y = rb.velocity.y;
            rb.velocity = correctedVel;
        }
    }

    void HandleMovement() {
        
        if(canMove) {
            //? MOVE
            var speed = isRunning ? runSpeed : walkSpeed;
            moveDirection = (transform.forward * xAxis) + (transform.right * yAxis);
            
            //? JUMP
            var mvDirY = moveDirection.y;

            // TODO: check canJump
            //moveDirection.y = jumpPressed && isGrounded ? jumpForce : mvDirY;
            // TODO: if (!grounded)
            //moveDirection.y -= (jumpPressed ? jumpGravity : fallGravity);

            // Apply forces
            rb.AddForce(moveDirection.normalized * speed, ForceMode.Force);
            

            // TODO: https://www.youtube.com/watch?v=qQLvcS9FxnY
            // TODOv2: https://www.youtube.com/watch?v=f473C43s8nE
        }
    }
}