using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

[RequireComponent(typeof(CharacterController), typeof(PlayerController))]
public class PlayerMovement : NetworkBehaviour {
    // ==================== Configuration ====================
    [Header("Camera Movement / Look")]
    [SerializeField] GameObject cameraHandle;
    [SerializeField] GameObject weaponHandle;
    [SerializeField, Range(0f, 90f)] float lookMaxAngleUp   = 80f;
    [SerializeField, Range(0f, 90f)] float lookMaxAngleDown = 90f;

    [Header("Character Movement")]
    [SerializeField] float walkSpeed = 12f;
    [SerializeField] float runSpeed  = 20f;

    [Header("Character Jump")]
    [SerializeField] LayerMask jumplableLayers;
    [SerializeField] float groundCheckRange = 0.3f;
    [SerializeField, Range(1f, 20f)] float gravityScale = 4f;
    [SerializeField] float jumpHeigh = 2f;

    // ====================== References =====================
    CharacterController characterController;
    PlayerController controller;

    // ====================== Variables ======================
    Vector3 _moveDirection = Vector3.zero, _velocity = Vector3.zero;
    float _cameraPitch = 0;
    bool _isGrounded;

    // ======================= NetCode ========================
    public override void OnStartNetwork() {
        characterController = GetComponent<CharacterController>();
        controller = GetComponent<PlayerController>();
    }

    public override void OnStartClient() {
        base.OnStartClient();
    }

    // ====================== Unity Code ======================
    void OnDrawGizmosSelected() {
        var feetPos = transform.position;
        feetPos.y -= characterController ? characterController.height / 2 : 0;
        Gizmos.DrawWireSphere(feetPos, groundCheckRange);
    }

    void Update() {
        if (!IsOwner) return;
        if (controller.CanMove) {
            UpdateState();

            HandleCameraLook();
            HandleMovement();

            ApplyMovementForces();
        }
    }


    // ===================== Custom Code =====================
    void UpdateState() {
        var feetPos = transform.position;
        feetPos.y -= characterController.height / 2;

        _isGrounded = Physics.CheckSphere(feetPos, groundCheckRange, jumplableLayers);
    }

    void HandleCameraLook() {
        if (controller.CanControl) {
            var lookDelta = InputManager.InGame_LookDelta;
            _cameraPitch -= lookDelta.y * SettingsManager.LookSensitivityY;
            _cameraPitch = Mathf.Clamp(_cameraPitch, -lookMaxAngleUp, lookMaxAngleDown);

            cameraHandle.transform.localRotation = Quaternion.Euler(_cameraPitch, 0, 0);
            weaponHandle.transform.localRotation = Quaternion.Euler(_cameraPitch, 0, 0);
            transform.rotation *= Quaternion.Euler(0, lookDelta.x * SettingsManager.LookSensitivityX, 0);
        }
    }

    void HandleMovement() {
        var speed = InputManager.InGame_RunPressed ? runSpeed : walkSpeed;
        // User input only considered if player can control
        var moveAxis = controller.CanControl ? InputManager.InGame_Movement : Vector2.zero;


        var mY = _moveDirection.y;
        _moveDirection = ( transform.forward * moveAxis.y ) + ( transform.right * moveAxis.x );
        _moveDirection *= speed; // Scale with current move speed
        _moveDirection.y = mY;
    }

    void ApplyMovementForces() {
        var g = Physics.gravity.y * gravityScale;
        var jumpPressed = controller.CanControl && InputManager.InGame_JumpPressed;

        // Apply movement
        characterController.Move(_moveDirection * Time.deltaTime);

        //! Vertical Movements + Gravity
        if (_isGrounded && jumpPressed) {
            // Height to Velocity Formula => v = sqrt(H * -2g)
            _velocity.y = Mathf.Sqrt(jumpHeigh * ( -2 * g ));
        }
        else {
            // Time.deltaTime applied twice on gravity because of freeFall formula => deltaY = (1/2) * g * t^2
            // Acceleration split in two to average out frame inconsistencies
            _velocity.y += g * Time.deltaTime / 2;

            // Cancel velocity when grounded
            if (_isGrounded) _velocity.y = -0.5f;
        }

        characterController.Move(_velocity * Time.deltaTime);
        // Apply the rest of the acceleration
        _velocity.y += g * Time.deltaTime / 2;
    }


    [ServerRpc(RequireOwnership = false)]
    public void RequestTeleport(Vector3 position) {
        Teleport(base.Owner, position);
    }

    [TargetRpc]
    void Teleport(NetworkConnection target, Vector3 position) {
        characterController.enabled = false;
        transform.position = position;
        characterController.enabled = true;
    }
}