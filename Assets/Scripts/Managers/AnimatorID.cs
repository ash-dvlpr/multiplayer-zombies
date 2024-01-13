using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimatorID {

    // Entities
    public static readonly int isAlive = Animator.StringToHash("IsAlive");
    //public static readonly int isMoving = Animator.StringToHash("IsMoving");
    
    public static readonly int isRunning = Animator.StringToHash("IsRunning");
    
    public static readonly int triggerAttack = Animator.StringToHash("TriggerAttack");


    // Fade effects
    public static readonly int forcedFade = Animator.StringToHash("ForcedFade");
    public static readonly int triggerFadeOut = Animator.StringToHash("TriggerFadeOut");
    public static readonly int triggerFadeIn = Animator.StringToHash("TriggerFadeIn");

}
