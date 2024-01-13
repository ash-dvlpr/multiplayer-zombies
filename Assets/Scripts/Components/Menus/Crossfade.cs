using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Crossfade : AMenu {
    public override MenuID MenuKey { get => MenuID.Crossfade; }


    // ====================== References =====================
    Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();
    }

    // ===================== Custom Code =====================
    private void ResetTriggers() {
        animator.ResetTrigger(AnimatorID.triggerFadeOut);
        animator.ResetTrigger(AnimatorID.triggerFadeIn);
    }
    public void TriggerFadeOut(bool forced = false) {
        ResetTriggers();
        animator.SetBool(AnimatorID.forcedFade, forced);
        animator.SetTrigger(AnimatorID.triggerFadeOut);
    }
    public void TriggerFadeIn(bool forced = false) {
        ResetTriggers();
        animator.SetBool(AnimatorID.forcedFade, forced);
        animator.SetTrigger(AnimatorID.triggerFadeIn);
    }
}
