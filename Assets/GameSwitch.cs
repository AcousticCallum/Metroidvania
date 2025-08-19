using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class GameSwitch : MonoBehaviour
{
    [SerializeField] UnityEvent onInteract;

    public void Interact()
    {
        if (onInteract != null) onInteract.Invoke();
    }
}
