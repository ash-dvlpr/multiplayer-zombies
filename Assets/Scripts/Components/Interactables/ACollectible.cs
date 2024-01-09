using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using System;

/// <summary>
/// Defines a Resource Collectible
/// </summary>
/// <typeparam name="R">Target AResource</typeparam>
public abstract class ACollectible<R> : ABaseCollectible where R : AResource {
    public override Type Target { get => typeof(R); }

    /// <summary>
    /// Base implementetion for collection of the object.<br></br>
    /// </summary>
    /// <param name="interactor"></param>
    [Server]
    public override void Interact(IInteractor interactor) {
        if (interactor.TryGet<R>(out var resource)) {
            Collect(resource);
        }
    }

    /// <summary>
    /// Defines the outcome of collecting this object. Base implementation despawns the object.<br></br>
    /// 
    /// NOTE: Implementor should define the [<see cref="ServerAttribute">Server</see>] attribute.
    /// </summary>
    /// <param name="interactorResource">Resource that will be affected by the interaction.</param>
    [Server]
    public virtual void Collect(R interactorResource) {
        base.Despawn(this.gameObject);
    }
}

