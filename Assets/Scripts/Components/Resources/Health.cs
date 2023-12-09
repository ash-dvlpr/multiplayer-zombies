using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : AResource {
    // ==================== Configuration ====================
    public override ResourceType ResType { get => ResourceType.Plentiful; }

    // ====================== Variables ======================
    public bool IsAlive { 
        get => (Amount > 0);
    }

    // ====================== Unity Code ======================
    protected override void Awake() {
        base.Awake();
    }

    // ===================== Custom Code =====================
    //[ServerRpc(RequireOwnership = false)]
    [Server]
    public void Heal(int amount) {
        Amount += Math.Max(0, amount);
    }

    //[ServerRpc(RequireOwnership = false)]
    [Server]
    public void Damage(int amount) {
        Debug.Log($"Got hit for {amount} -> now got {Amount} hp");
        Amount -= Math.Max(0, amount);
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
