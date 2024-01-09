using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class AmmoPack : ACollectible<Ammo> {
    // ==================== Configuration ====================
    [field: Header("Configuration")]
    [SerializeField] bool restoreAll = true;
    [SerializeField, Min(1)] int ammoAmount = 10;

    // ======================= NetCode =======================
    [Server]
    public override void Collect(Ammo interactorResource) {
        var amount = restoreAll ? interactorResource.Max : ammoAmount;
        interactorResource.Reload(amount);
    }
}