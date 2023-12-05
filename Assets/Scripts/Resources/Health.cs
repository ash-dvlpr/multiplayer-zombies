using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : Resource {
    // ==================== Configuration ====================
    public override ResourceType ResType { get => ResourceType.Clasic; }

    // ====================== Unity Code ======================
    void Awake() {
        base.Reset();   
    }
}
