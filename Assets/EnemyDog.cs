using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDog : Enemy
{
    [Space]

    public float moveSpeed;
    public float moveControl;
    public float stopControl;

    [SerializeField] private float jumpHeight;

    protected override void DetectMove()
    {
        float targetVelocityX = Mathf.Sign(toPlayer.x) * moveSpeed;
        float control = targetVelocityX * rb.velocityX < 0.0f ? stopControl : moveControl;
        rb.velocityX = Mathf.Lerp(rb.velocityX, targetVelocityX, control * Time.deltaTime);

        if (!Grounded())
        {
            return;
        }

        if(toPlayer.y > 0.0f && Mathf.Abs(toPlayer.x) < 2.5f)
        {
            rb.velocityY = Mathf.Sqrt(Mathf.Min(toPlayer.y, jumpHeight) * -Physics2D.gravity.y * 2.0f);
        }
    }

    protected override void DefaultMove()
    {
        rb.velocityX = Mathf.Lerp(rb.velocityX, 0.0f, stopControl * Time.deltaTime);
    }

    protected override void UpdateAnims()
    {
        //float signedAngle = Vector2.SignedAngle(Vector2.right, rb.velocity.normalized);
        //signedAngle = Mathf.Clamp(signedAngle, -15.0f, 15.0f);

        //rend.transform.rotation = Quaternion.Euler(signedAngle * Vector3.forward);
        rend.transform.rotation = Quaternion.identity;
        if (rb.velocityX < 0.0f) rend.transform.Rotate(180.0f * Vector3.right, Space.Self);
    }

    private bool Grounded()
    {
        if (rb.velocityY > 0.01f) return false;

        RaycastHit2D hit = Physics2D.CircleCast(rb.position, 0.35f, Vector2.down, 0.1f, groundMask);

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(rb.position, rb.position + 2.5f * Vector2.right);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(rb.position, 0.35f);
        Gizmos.DrawWireSphere(rb.position + 0.1f * Vector2.down, 0.35f);
    }
}
