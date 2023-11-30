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
}
