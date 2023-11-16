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
    [SerializeField] private float groundCheckRange = 2.2f;
    [SerializeField] private float jumpHeigh        = 2f;

    //? Variables
    private bool isGrounded;
    private Vector3 moveDirection = Vector3.zero, velocity = Vector3.zero;
    private float cameraPitch = 0;

    private bool jumpPressed, runPressed;
    private float xAxis = 0f, yAxis = 0f, mouseX = 0f, mouseY = 0f;

    //? References
    [Header("Other Configuration")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject playerFeet;
    [SerializeField] private LayerMask groundLayer;
    CharacterController characterController;
    Rigidbody rb;

    // =======================================================
    void Start() {
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        canMove = true;
    }

    void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(playerFeet.transform.position, groundCheckRange);
    }

    void Update() {
        UpdateInputs();
        HandleCameraMovement();
        HandleMovement();
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

    void HandleMovement() {
        isGrounded = Physics.CheckSphere(playerFeet.transform.position, groundCheckRange, groundLayer);
        // Cancel falling velocity when grounded
        if(isGrounded && rb.velocity.y < 0.2f) velocity.y = -2f;

        //! Horizontal movements
        var speed = runPressed ? runSpeed : walkSpeed;
        moveDirection = (transform.forward * xAxis) + (transform.right * yAxis);
        moveDirection *= speed; // Scale with current move speed

        // Time.deltaTime makes it framerate independent
        characterController.Move(moveDirection * Time.deltaTime);

        //! Vertical Movements + Gravity
        if (jumpPressed && isGrounded) {
            // Height to Velocity Formula => v = sqrt(H * -2g)
            velocity.y = Mathf.Sqrt(jumpHeigh * -2f * Physics.gravity.y);
        }

        // Time.deltaTime applied twice on gravity because of freeFall formula => deltaY = (1/2) * g * t^2
        velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}