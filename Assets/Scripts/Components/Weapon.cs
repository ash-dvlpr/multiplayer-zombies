using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Weapon : MonoBehaviour {

    // ==================== Configuration ====================
    [field: Header("Combat")]
    [field: SerializeField] public float MaxShotDistance { get; private set; } = 30;
    [field: SerializeField] public float ShootingDelay { get; private set; } = 0.1f;
    [field: SerializeField] public int ShotDamage { get; private set; } = 20;
    [SerializeField] AudioClip shotSound;


    // ====================== References =====================

    [field: Header("Hands Rigging")]
    [field: SerializeField] public Transform LeftHandPos { get; private set; }
    [field: SerializeField] public Transform RightHandPos { get; private set; }

    [field: Header("Effects")]
    [field: SerializeField] public Transform MuzzlePos { get; private set; }
    [SerializeField] VisualEffect shootingVFX;

    // ===================== Custom Code ======================
    public void PlayShootEffects() {
        if (shotSound != null)
            AudioManager.PlayClipAt(shotSound, this.transform.position);
        if (shootingVFX != null)
            shootingVFX.Play();
    }
}
