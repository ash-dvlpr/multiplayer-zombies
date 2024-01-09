using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class HealthPack : ACollectible<Health> {
    // ==================== Configuration ====================
    [field: Header("Configuration")]
    [SerializeField] bool restoreAll = false;
    [SerializeField, Min(1)] int healthAmount = 60;

    // ======================= NetCode =======================
    [Server]
    public override void Collect(Health interactorResource) {
        var amount = restoreAll ? interactorResource.Max : healthAmount;
        interactorResource.Heal(amount);

        base.Collect(interactorResource);
    }
}