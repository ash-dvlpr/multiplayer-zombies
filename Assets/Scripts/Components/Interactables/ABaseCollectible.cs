using FishNet.Object;
using System;
using UnityEngine;

public abstract class ABaseCollectible : NetworkBehaviour, IInteractable {
    // ==================== Configuration ====================
    [Header("Sounds")]
    [SerializeField] protected AudioClip pickupSound;

    // ====================== References =====================
    protected AudioSource audioSource;


    // ===================== Custom Code =====================
    public override void OnStartClient() {
        base.OnStartClient();

        audioSource = this.gameObject.AddComponent<AudioSource>();
    }

    public abstract Type Target { get; }
    public abstract void Interact(IInteractor interactor);


    // ======================= Sounds ========================
    [ObserversRpc]
    public void PlayPickupSound() {
        if (base.IsClient && pickupSound != null)
            AudioManager.PlayClipOn(pickupSound, audioSource);
    }
}