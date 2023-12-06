using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponRigHandler : MonoBehaviour {
    [Header("Config")]
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private TwoBoneIKConstraint rightHandIK;
    [SerializeField] private Weapon weapon;

    private RigBuilder rigBuilder;

    private void Awake() {
        rigBuilder = GetComponent<RigBuilder>();
    }

    void Start() {
        UpdateHandsRig();
    }

    public void UpdateHandsRig() {
        leftHandIK.data.target = weapon.LeftHandPos;
        rightHandIK.data.target = weapon.RightHandPos;
        rigBuilder.Build();
    }
}
