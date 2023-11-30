using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(ICharacterInput))]
public class PlayerMovement : MonoBehaviour {
    // ==================== Configuration ====================
    [Header("Camera Movement / Look")]
    [SerializeField] GameObject cameraHandle;
    [SerializeField] float lookSpeedX = 24f;
    [SerializeField] float lookSpeedY = 24f;
    [SerializeField][Range(0f, 90f)] float lookMaxAngleUp   = 80f;
    [SerializeField][Range(0f, 90f)] float lookMaxAngleDown = 90f;


    [Header("Character Movement")]
    [SerializeField] float walkSpeed = 12f;
    [SerializeField] float runSpeed  = 20f;
    [SerializeField] float gravity   = 1.5f;

    [SerializeField] LayerMask jumplableLayers;
    [SerializeField] private float groundCheckRange = 2.2f;

    // ====================== References =====================
    CharacterController _characterController;
    ICharacterInput _inputs;
    //Camera _camera;

    // ====================== Variables ======================
    [SerializeField] bool canMove = true;

    Vector3 _moveDirection = Vector3.zero, _velocity = Vector3.zero;
    float _cameraPitch = 0;
    bool _isGrounded;


    // ====================== Unity Code ======================
    void Awake() {
        _characterController = GetComponent<CharacterController>();
        _inputs = GetComponent<ICharacterInput>();
        //_camera = GetComponentInChildren<Camera>();
    }

    //void OnDrawGizmosSelected() {
        //Gizmos.DrawWireSphere(playerFeet.transform.position, groundCheckRange);
    //}

    void Update() {
        //if (canMove) {
        //    HandleCameraLook();
        //    HandleMovement();
        //    HandleFinalMovements();
        //}
    }


    // ===================== Custom Code =====================
    void HandleCameraLook() {
        //_cameraPitch -= _inputs.MouseY * lookSpeedY;
        //_cameraPitch = Mathf.Clamp(_cameraPitch, -lookMaxAngleUp, lookMaxAngleDown);
        //_camera.transform.localRotation = Quaternion.Euler(_cameraPitch, 0, 0);
        //transform.rotation *= Quaternion.Euler(0, _inputs.MouseX * lookSpeedX, 0);
    }

    void HandleMovement() {


    }

    void HandleFinalMovements() {

    } 
}