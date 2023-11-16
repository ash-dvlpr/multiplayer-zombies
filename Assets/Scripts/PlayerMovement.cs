using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerController), typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {
    [Header("Camera")]
    [SerializeField] private float lookSpeed    = 24f;
    [SerializeField] private float lookMaxAngle = 80f;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed  = 10f;
    [SerializeField] private float moveForce = 20f;

    [Header("Jump")]
    [SerializeField] private float jumpForce   = 10f;
    [SerializeField] private float maxJumpTime = 10f;
    [SerializeField] private float jumpGravity = 1f;
    [SerializeField] private float fallGravity = 3f;
    [SerializeField] private float groundCheckRange = 2.2f;

    //? Variables
    private bool canMove = true, isGrounded, isJumping;
    private Vector3 moveDirection = Vector3.zero;
    private float cameraPitch = 0, jumpTimer;

    private bool jumpPressed, runPressed;
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
            HandleJump();
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

        var speed = runPressed ? runSpeed : walkSpeed;
        if (curVel.magnitude > speed) {
            var correctedVel = curVel.normalized * speed;
            correctedVel.y = rb.velocity.y;
            rb.velocity = correctedVel;
        }
    }

    void HandleMovement() {
        
        if(canMove) {
            //? MOVE
            moveDirection = (transform.forward * xAxis) + (transform.right * yAxis);

            // Apply forces
            var gravity = jumpPressed && isJumping ? jumpGravity : fallGravity;
            rb.AddForce(Physics.gravity * gravity * rb.mass);
            rb.AddForce(moveDirection.normalized * moveForce, ForceMode.Force);

            // TODO: https://www.youtube.com/watch?v=qQLvcS9FxnY
            // TODOv2: https://www.youtube.com/watch?v=f473C43s8nE
        }
    }

    void HandleJump() {
        //? JUMP
        if (jumpPressed && isGrounded) {
            // Start jumping 
            if (isGrounded) {
                rb.AddForce(transform.up * jumpForce * Time.deltaTime, ForceMode.Impulse);
                jumpTimer = maxJumpTime;
                isJumping = true;
            }
            // Make higher jump is input was held
            else if (isJumping) {
                if (jumpTimer > 0) {
                    
                    jumpTimer -= Time.deltaTime;
                }
                else isJumping = false;
            }
        }
        else isJumping = false;
    }
}