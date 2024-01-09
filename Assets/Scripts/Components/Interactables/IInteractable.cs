using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Interface that defines an object as being interactable by <see cref="IInteractor">Interactors</see> 
/// that <see cref="IInteractor.AllowedInteractableTypes">allow</see> it.<br></br>
/// 
/// Must define what <see cref="Target">Component/Class</see> they want to interact with.
/// </summary>
public interface IInteractable {
    public Type Target { get; }
    public void Interact(IInteractor interactor);
}