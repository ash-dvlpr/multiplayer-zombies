using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    // ====================== References =====================

    [field: Header("Hands Rigging")]
    [field: SerializeField] public Transform LeftHandPos { get; private set; }
    [field: SerializeField] public Transform RightHandPos { get; private set; }
    
    [field: Header("Effects")]
    [field: SerializeField] public Transform MuzzlePos { get; private set; }

    // ====================== Unity Code ======================

}
