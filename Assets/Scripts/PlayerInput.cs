using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerInput : MonoBehaviour, ICharacterInput {
    public ICharacterInput.InputStates State { get => _state; }
    private ICharacterInput.InputStates _state;
    
    void Update() {
        UpdateInputs();
    }

    public void UpdateInputs() {
        //? Camera
        _state.mouseX = Input.GetAxis("Mouse X");
        _state.mouseY = Input.GetAxis("Mouse Y");

        //? Movement
        _state.xAxis = Input.GetAxis("Vertical");
        _state.yAxis = Input.GetAxis("Horizontal");

        // Movement modifiers
        _state.jumpPressed = Input.GetButtonDown("Jump");
        _state.jumpHeld    = Input.GetButton("Jump");
        _state.runPressed  = Input.GetButtonDown("Run");
        _state.runHeld     = Input.GetButton("Run");
    }
}
