using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterInput {
    public struct InputStates {
        public bool jumpPressed, jumpHeld;
        public bool runPressed, runHeld;

        public float xAxis, yAxis, mouseX, mouseY;
    }
    public abstract InputStates State { get; }

    public bool JumpPressed { get => State.jumpPressed; }
    public bool JumpHeld { get => State.jumpHeld; }
    public bool RunPressed { get => State.runPressed; }
    public bool RunHeld { get => State.runHeld; }

    public float XAxis { get => State.xAxis; }
    public float YAxis { get => State.yAxis; }
    public float MouseX { get => State.mouseX; }
    public float MouseY { get => State.mouseY; }
}
