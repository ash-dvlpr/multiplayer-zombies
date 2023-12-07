using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponRigHandler : MonoBehaviour {
    
    // ====================== References =====================
    [Header("Config")]
    [SerializeField] TwoBoneIKConstraint leftHandIK;
    [SerializeField] TwoBoneIKConstraint rightHandIK;
    [SerializeField] Weapon weapon;

    RigBuilder rigBuilder;

    private void Awake() {
        rigBuilder = GetComponent<RigBuilder>();
    }

    // ====================== Unity Code ======================
    void Start() {
        UpdateHandsRig();
    }

    // ===================== Custom Code =====================
    public void UpdateHandsRig() {
        leftHandIK.data.target = weapon.LeftHandPos;
        rightHandIK.data.target = weapon.RightHandPos;
        rigBuilder.Build();
    }
}
