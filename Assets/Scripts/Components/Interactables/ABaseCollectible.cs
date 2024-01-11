using FishNet.Object;
using System;
using UnityEngine;

public abstract class ABaseCollectible : NetworkBehaviour, IInteractable {
    // ==================== Configuration ====================
    [Header("Sounds")]
    [SerializeField] AudioClip pickupSound;


    // ===================== Custom Code =====================

    public abstract Type Target { get; }
    public abstract void Interact(IInteractor interactor);


    // ======================= Sounds ========================
    [ObserversRpc]
    public void PlayPickupSound() {
        if (base.IsClient && pickupSound != null)
            AudioManager.PlayClipAt(pickupSound, this.transform.position);
    }
}