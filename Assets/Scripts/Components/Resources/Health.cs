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
    public void Heal(int amount) {
        Amount += Math.Max(0, amount);
    }

    public void Damage(int amount) {
        Amount -= Math.Max(0, amount);
    }

    // ================== Outside Facing API ==================
    protected override void TriggerOnChange() {
        base.TriggerOnChange();

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
