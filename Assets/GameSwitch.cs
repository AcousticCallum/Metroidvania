using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class GameSwitch : MonoBehaviour
{
    [SerializeField] UnityEvent onInteract;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && Vector2.Distance(transform.position, Player.instance.GetCenter()) <= 10.0f) Interact();
    }

    public void Interact()
    {
        if (onInteract != null) onInteract.Invoke();
    }
}
