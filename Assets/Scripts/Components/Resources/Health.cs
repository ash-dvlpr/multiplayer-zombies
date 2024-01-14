using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : AResource {
    // ==================== Configuration ====================
    public override ResourceType ResType { get => ResourceType.Plentiful; }

    [field: SerializeField] public ParticleSystem onHitParticleSystem;

    // ====================== Variables ======================
    public bool IsAlive { 
        get => (Amount > 0);
    }

    // ======================= NetCode ========================

    // ===================== Custom Code =====================
    [Server]
    public void Heal(int amount) {
        Amount += Math.Max(0, amount);
    }

    [Server]
    public void Damage(int amount) {
        Amount -= Math.Max(0, amount);
        PlayHitFX();
    }

    [ObserversRpc]
    public void PlayHitFX() {
        if (base.IsClient && onHitParticleSystem != null) { 
            onHitParticleSystem.Play();
        }
    }

    // ================== Outside Facing API ==================
    protected override void TriggerOnChange(int prev, int next, bool asServer) {
        base.TriggerOnChange(prev, next, asServer);

        if (Amount.Equals(0)) {
            onDeath?.Invoke();
        }
    }

    private event Action onDeath;
    public event Action OnDeath {
        add    { lock(this) { onDeath += value; } }
        remove { lock(this) { onDeath -= value; } }
    }
}
