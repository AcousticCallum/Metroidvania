using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDoor : MonoBehaviour
{
    public int switchesRequired;
    private int switchesActivated;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && Vector2.Distance(transform.position, Player.instance.GetCenter()) <= 10.0f) Interact();
    }

    public void Interact(bool undo = false)
    {
        switchesActivated += undo ? -1 : 1;

        if(switchesActivated >= switchesRequired)
        {
            gameObject.SetActive(false);
        }
    }
}
