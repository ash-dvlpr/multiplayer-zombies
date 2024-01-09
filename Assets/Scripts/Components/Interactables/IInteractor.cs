using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface that defines an object as being capable of interacting with 
/// <see cref="IInteractable{T}">Interactables</see> of the same type.<br></br>
/// 
/// The interactor object must expose the Component to be affected by the result of the interaction.
/// 
/// </summary>
/// <typeparam name="T">Target Component</typeparam>
public interface IInteractor<T> {
    public T Value { get; }
    public void TryInteract(IInteractable<T> interactable);
}

