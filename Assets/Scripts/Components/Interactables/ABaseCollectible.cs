using FishNet.Object;
using System;

public abstract class ABaseCollectible : NetworkBehaviour, IInteractable {
    public abstract Type Target { get; }
    public abstract void Interact(IInteractor interactor);
}