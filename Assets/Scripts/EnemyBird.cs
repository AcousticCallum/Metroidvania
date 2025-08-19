using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBird : Enemy
{
    [Space]

    public float moveSpeed;
    public float moveControl;
    public float stopControl;

    protected override void DetectMove()
    {
        Vector2 targetVelocity = toPlayer.normalized * moveSpeed;
        float control = Vector2.Dot(targetVelocity, rb.velocity) < 0.0f ? stopControl : moveControl;
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, control * Time.deltaTime);
    }

    protected override void DefaultMove()
    {
        rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, stopControl * Time.deltaTime);
    }

    protected override void UpdateAnims()
    {
        float signedAngle = Vector2.SignedAngle(Vector2.right, rb.velocity.normalized);

        rend.transform.rotation = Quaternion.Euler(signedAngle * Vector3.forward);
        if (rb.velocityX < 0.0f) rend.transform.Rotate(180.0f * Vector3.right, Space.Self);
    }
}
