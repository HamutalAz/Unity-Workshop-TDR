using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string promptMessage;

    public void BaseInteract()
    {
        Interact();
    }

    protected virtual void Interact()
    {

    }

    public virtual void UpdateUI(Dictionary<string, object> data)
    {

    }

    public virtual void sendCode(string code)
    {

    }
}
