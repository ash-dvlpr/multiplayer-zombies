using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    [field: Header("Hands Rigging")]
    [field: SerializeField] public Transform LeftHandPos { get; private set; }
    [field: SerializeField] public Transform RightHandPos { get; private set; }
    
    [field: Header("Effects")]
    [field: SerializeField] public Transform MuzzlePos { get; private set; }



    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
