using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public float destroyThreshold;
    public bool dashOnly;
    public float bounciness;

    [Space]

    [SerializeField] GameSwitch gameSwitch;

    public bool TryDestruct(float speed)
    {
        Debug.Log($"Destructible hit with speed: {speed}");

        if (speed >= destroyThreshold && Health.TryDamage(gameObject))
        {
            if(Health.IsDead(gameObject) && gameSwitch) gameSwitch.Interact();

            return true;
        }

        return false;
    }
}
