using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] Transform remains;

    [Space]

    public float destroyThreshold;
    public bool dashOnly;
    public float bounciness;

    private bool destructed;

    [Space]

    [SerializeField] GameSwitch gameSwitch;

    public bool TryDestruct(float speed)
    {
        Debug.Log($"Destructible hit with speed: {speed}");

        if (speed >= destroyThreshold && Health.TryDamage(gameObject, 1.0f, Player.instance.gameObject))
        {
            return true;
        }

        return false;
    }

    private void OnDestruct()
    {
        if (destructed) return;

        if (gameSwitch) gameSwitch.Interact();

        if (remains) remains.SetParent(null);

        destructed = true;
    }

    public void OnDisable()
    {
        if (Health.IsDead(gameObject)) OnDestruct();
    }

    public void OnDestroy()
    {
        if (Health.IsDead(gameObject)) OnDestruct();
    }
}
