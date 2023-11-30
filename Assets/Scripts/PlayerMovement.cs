using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController), typeof(ICharacterInput))]
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
    [SerializeField] private float fallGravity      = 1.5f;

    //? References
    [Header("Other Configuration")]
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject playerFeet;
    [SerializeField] private LayerMask groundLayer;
    CharacterController characterController;
    ICharacterInput inputs;
    Rigidbody rb;

    //? Variables
    private bool isGrounded;
    private Vector3 moveDirection = Vector3.zero, velocity = Vector3.zero;
    private float cameraPitch = 0;


    // =======================================================
    void Start() {
        characterController = GetComponent<CharacterController>();
        inputs = GetComponent<ICharacterInput>();
        rb = GetComponent<Rigidbody>();
    }

    void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(playerFeet.transform.position, groundCheckRange);
    }

    void Update() {
        HandleCameraMovement();
        HandleMovement();
    }

    // =======================================================
    void HandleCameraMovement() {
        cameraPitch -= inputs.State.mouseY * lookSpeed;
        cameraPitch = Mathf.Clamp(cameraPitch, -lookMaxAngle, lookMaxAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        transform.rotation *= Quaternion.Euler(0, inputs.State.mouseX * lookSpeed, 0);
    }

    void HandleMovement() {
        isGrounded = Physics.CheckSphere(playerFeet.transform.position, groundCheckRange, groundLayer);
        var gravity = !isGrounded && !inputs.State.jumpPressed ? Physics.gravity.y * fallGravity : Physics.gravity.y;

        // Cancel excesive falling velocity when grounded
        if(isGrounded && velocity.y < Physics.gravity.y) velocity.y = Physics.gravity.y;

        //! Horizontal movements
        var speed = inputs.State.runPressed ? runSpeed : walkSpeed;
        moveDirection = (transform.forward * inputs.State.xAxis) + (transform.right * inputs.State.yAxis);
        moveDirection *= speed; // Scale with current move speed

        // Time.deltaTime makes it framerate independent
        characterController.Move(moveDirection * Time.deltaTime);

        //! Vertical Movements + Gravity
        if (inputs.State.jumpPressed && isGrounded) {
            // Height to Velocity Formula => v = sqrt(H * -2g)
            velocity.y = Mathf.Sqrt(jumpHeigh * -2f * gravity);
        }

        // Time.deltaTime applied twice on gravity because of freeFall formula => deltaY = (1/2) * g * t^2
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}