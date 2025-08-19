using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDoor : MonoBehaviour
{
    public int switchesRequired;
    private int switchesActivated;

    public void Interact(bool undo = false)
    {
        switchesActivated += undo ? -1 : 1;

        if(switchesActivated >= switchesRequired)
        {
            gameObject.SetActive(false);
        }
    }
}
