using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {
    [Header("Camera")]
    [SerializeField] private float lookSpeed    = 24f;
    [SerializeField] private float lookMaxAngle = 80f;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 12f;
    [SerializeField] private float runSpeed  = 20f;

    [Header("Jump")]
    [SerializeField] private float jumpForce   = 10f;
    [SerializeField] private float maxJumpTime = 10f;
    [SerializeField] private float jumpGravity = 1f;
    [SerializeField] private float fallGravity = 3f;
    [SerializeField] private float groundCheckRange = 2.2f;

    //? Variables
    private bool canMove = true, isGrounded, isJumping;
    private Vector3 moveDirection = Vector3.zero, velocity = Vector3.zero;
    private float cameraPitch = 0, jumpTimer;

    private bool jumpPressed, runPressed;
    private float xAxis = 0f, yAxis = 0f, mouseX = 0f, mouseY = 0f;

    //? References
    [Header("Other Configuration")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private LayerMask groundLayer;
    CharacterController characterController;
    Rigidbody rb;

    // =======================================================
    void Start() {
        characterController = GetComponent<CharacterController>();
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
            //HandleJump();
            HandleMovement();
            //FixMovement();
        }
    }

    void FixedUpdate() {
        
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
        cameraPitch -= mouseY * lookSpeed;
        cameraPitch = Mathf.Clamp(cameraPitch, -lookMaxAngle, lookMaxAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        transform.rotation *= Quaternion.Euler(0, mouseX * lookSpeed, 0);
    }

    void FixMovement() {

    }

    void HandleMovement() {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckRange, groundLayer);

        // Cancel falling velocity when grounded
        if(isGrounded) velocity.y = Physics.gravity.y;

        var speed = runPressed ? runSpeed : walkSpeed;
        moveDirection = (transform.forward * xAxis) + (transform.right * yAxis);
        moveDirection *= speed; // Scale with current move speed

        // Time.deltaTime makes it framerate independent
        characterController.Move(moveDirection * Time.deltaTime);

        // Apply Gravity (Time.deltaTime applied twice because of freeFall formula -> t^2)
        velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleJump() {

    }
}