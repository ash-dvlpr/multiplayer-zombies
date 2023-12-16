using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : AResource {
    // ==================== Configuration ====================
    public override ResourceType ResType { get => ResourceType.Plentiful; }

    // ====================== Variables ======================
    public bool HasAmmo { 
        get => (Amount > 0);
    }

    // ======================= NetCode ========================

    // ===================== Custom Code =====================
    [Server]
    public void Reload(int amount) {
        Amount += Math.Max(0, amount);
    }

    [Server]
    public void Consume(int amount) {
        Amount -= Math.Max(0, amount);
    }

    // ================== Outside Facing API ==================
    protected override void TriggerOnChange(int prev, int next, bool asServer) {
        base.TriggerOnChange(prev, next, asServer);

        if (Amount.Equals(0)) {
            onNoAmmoLeft?.Invoke();
        }
    }

    private event Action onNoAmmoLeft;
    public event Action OnNoAmmoLeft {
        add    { lock(this) { onNoAmmoLeft += value; } }
        remove { lock(this) { onNoAmmoLeft -= value; } }
    }
}
