using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FishNet.Object;
using FishNet.Serializing;

/// <summary>
/// Interface that defines an object as being capable of interacting with 
/// <see cref="IInteractor.AllowedInteractableTypes">allowed</see> <see cref="IInteractable">Interactables</see>.<br></br>
/// 
/// The interactor object must expose the Component to be affected by the result of the interaction.
/// 
/// </summary>
/// <typeparam name="T">Target Component</typeparam>
public interface IInteractor : ITypeCache {
    public List<Type> AllowedInteractableTypes { get; }
    public bool CanInteract(IInteractable interactable) {
        return AllowedInteractableTypes.Contains(interactable.Target);
    }
    public void Interact(IInteractable interactable);
}