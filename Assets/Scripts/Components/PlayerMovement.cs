using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour {
    // ==================== Configuration ====================
    [Header("Camera Movement / Look")]
    [SerializeField] GameObject cameraHandle;
    [SerializeField] GameObject weaponHandle;
    [SerializeField] float lookSpeedX = 24f;
    [SerializeField] float lookSpeedY = 24f;
    [SerializeField, Range(0f, 90f)] float lookMaxAngleUp   = 80f;
    [SerializeField, Range(0f, 90f)] float lookMaxAngleDown = 90f;

    [Header("Character Movement")]
    [SerializeField] float walkSpeed = 12f;
    [SerializeField] float runSpeed  = 20f;

    [Header("Character Jump")]
    [SerializeField] LayerMask jumplableLayers;
    [SerializeField] private float groundCheckRange = 0.3f;
    [SerializeField, Range(1f, 20f)] float gravityScale = 4f;
    [SerializeField] float jumpHeigh = 2f;

    // ====================== References =====================
    CharacterController _characterController;

    // ====================== Variables ======================
    [SerializeField] bool canMove = true;

    Vector3 _moveDirection = Vector3.zero, _velocity = Vector3.zero;
    float _cameraPitch = 0;
    bool _isGrounded;


    // ====================== Unity Code ======================
    void Awake() {
        _characterController = GetComponent<CharacterController>();
    }

    void OnDrawGizmosSelected() {
        var feetPos = transform.position;
        feetPos.y -= _characterController ? _characterController.height / 2 : 0;
        Gizmos.DrawWireSphere(feetPos, groundCheckRange);
    }

    void Update() {
        if (canMove) {
            UpdateState();

            HandleCameraLook();
            HandleMovement();

            ApplyMovementForces();
        }
    }


    // ===================== Custom Code =====================
    void UpdateState() {
        var feetPos = transform.position;
        feetPos.y -= _characterController.height / 2;

        _isGrounded = Physics.CheckSphere(feetPos, groundCheckRange, jumplableLayers);
    }

    void HandleCameraLook() {
        var lookDelta = InputManager.InGameLookDelta;
        _cameraPitch -= lookDelta.y * lookSpeedY;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -lookMaxAngleUp, lookMaxAngleDown);

        cameraHandle.transform.localRotation = Quaternion.Euler(_cameraPitch, 0, 0);
        weaponHandle.transform.localRotation = Quaternion.Euler(_cameraPitch, 0, 0);
        transform.rotation *= Quaternion.Euler(0, lookDelta.x * lookSpeedX, 0);
    }

    void HandleMovement() {
        var speed = InputManager.InGameRunPressed ? runSpeed : walkSpeed;
        var moveAxis = InputManager.InGameMovement;

        var mY = _moveDirection.y;
        _moveDirection = (transform.forward * moveAxis.y) + (transform.right * moveAxis.x);
        _moveDirection *= speed; // Scale with current move speed
        _moveDirection.y = mY;
    }

    void ApplyMovementForces() {
        var g = Physics.gravity.y * gravityScale;

        // Apply movement
        _characterController.Move(_moveDirection * Time.deltaTime);

        //! Vertical Movements + Gravity
        if (_isGrounded && InputManager.InGameJumpPressed) {
            // Height to Velocity Formula => v = sqrt(H * -2g)
            _velocity.y = Mathf.Sqrt(jumpHeigh * (-2*g));
        } else {
            // Time.deltaTime applied twice on gravity because of freeFall formula => deltaY = (1/2) * g * t^2
            // Acceleration split in two to average out frame inconsistencies
            _velocity.y += g * Time.deltaTime / 2;

            // Cancel velocity when grounded
            if(_isGrounded) _velocity.y = -0.5f;
        }

        _characterController.Move(_velocity * Time.deltaTime);
        // Apply the rest of the acceleration
        _velocity.y += g * Time.deltaTime / 2;
    }
}