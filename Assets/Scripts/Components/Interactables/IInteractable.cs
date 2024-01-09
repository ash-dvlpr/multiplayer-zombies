using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Interface that defines an object as being interactable by 
/// <see cref="IInteractor{T}">Interactors</see> of the same type.
/// </summary>
/// <typeparam name="T">Target Component</typeparam>
public interface IInteractable<T> {
    public void Interact(IInteractor<T> interactor);
}