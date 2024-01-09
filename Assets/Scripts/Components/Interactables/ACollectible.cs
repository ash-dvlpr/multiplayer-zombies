using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

/// <summary>
/// Defines a Resource Collectible
/// </summary>
/// <typeparam name="R">Target AResource</typeparam>
public abstract class ACollectible<R> : NetworkBehaviour, IInteractable<R> where R : AResource  {

    /// <summary>
    /// Base implementetion for collection of the object.<br></br>
    /// 
    /// NOTE: Implementor should define the [<see cref="ServerRpcAttribute">ServerRpc(RequireOwnership = false)</see>] attribute.
    /// </summary>
    /// <param name="interactorResource">Resource that will be affected by the interaction.</param>
    [ServerRpc(RequireOwnership = false)]
    public virtual void Interact(IInteractor<R> interactor) {
        Collect(interactor.Value);
    }

    /// <summary>
    /// Defines the outcome of collecting this object.<br></br>
    /// 
    /// NOTE: Implementor should define the [<see cref="ServerAttribute">Server</see>] attribute.
    /// </summary>
    /// <param name="interactorResource">Resource that will be affected by the interaction.</param>
    public abstract void Collect(R interactorResource);
}

