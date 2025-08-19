using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDog : Enemy
{
    [Space]

    public Animator animator;

    [Space]

    public float moveSpeed;
    public float moveControl;
    public float stopControl;

    [SerializeField] private float jumpHeightMax;
    [SerializeField] private float jumpHeightMin;
    [SerializeField] private float jumpHeightOffset;
    [SerializeField] private float jumpChargeDistance;
    [SerializeField] private float jumpChargeDuration;
    public float jumpChargeStopControl;
    private float jumpChargeTimer;
    private float jumpDirection;
    private bool jumpCharging;
    private bool jumpingOverWall;

    protected override void DetectMove()
    {
        float targetVelocityX = Mathf.Sign(toPlayer.x) * moveSpeed;
        float control = targetVelocityX * rb.velocityX < 0.0f ? stopControl : moveControl;
        rb.velocityX = Mathf.Lerp(rb.velocityX, targetVelocityX, control * Time.deltaTime);

        if (!Grounded())
        {
            jumpCharging = false; // Cancel jump

            return;
        }

        if (!jumpCharging)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Mathf.Sign(toPlayer.x) * Vector2.right, 1.0f, groundMask);
            if (hit || Mathf.Abs(toPlayer.x) < jumpChargeDistance) // Jump conditions
            {
                Debug.Log("Jump charging");

                jumpChargeTimer = jumpChargeDuration;
                jumpDirection = Mathf.Sign(toPlayer.x);
                jumpingOverWall = hit;

                jumpCharging = true;
            }
            
            // Always return so anims update
            return;
        }

        // Jump charging logic

        jumpChargeTimer = Mathf.Max(jumpChargeTimer - Time.deltaTime, 0.0f);
        if (jumpChargeTimer == 0.0f) // Jump if timer reaches zero
        {
            Debug.Log("Jump started");

            rb.velocityX = jumpDirection * moveSpeed;

            float jumpHeight = jumpingOverWall ? jumpHeightMax : Mathf.Clamp(toPlayer.y + jumpHeightOffset, jumpHeightMin, jumpHeightMax); // Max jump height if there's a wall
            rb.velocityY = Mathf.Sqrt(jumpHeight * -Physics2D.gravity.y * 2.0f);

            if(!jumpingOverWall) stunTimer = stunDuration; // Stun for a short time after jumping (only if there is not a wall) 

            jumpCharging = false;

            return;
        }

        rb.velocityX = Mathf.Lerp(rb.velocityX, 0.0f, jumpChargeStopControl * Time.deltaTime);
    }

    protected override void DefaultMove()
    {
        rb.velocityX = Mathf.Lerp(rb.velocityX, 0.0f, stopControl * Time.deltaTime);
    }

    protected override void UpdateAnims()
    {
        animator.SetFloat("MoveSpeed", Mathf.Abs(rb.velocityX));
        animator.SetBool("JumpCharging", jumpCharging);
        animator.SetBool("Jumping", !Grounded());

        if (jumpCharging) return;

        rend.transform.rotation = Quaternion.identity;
        if (rb.velocityX < 0.0f) rend.transform.Rotate(180.0f * Vector3.up, Space.World);
    }

    private bool Grounded()
    {
        if (rb.velocityY > 0.01f) return false;

        RaycastHit2D hit = Physics2D.CircleCast(rb.position + 0.35f * Vector2.right, 0.35f, Vector2.down, 0.1f, groundMask);
        RaycastHit2D hit2 = Physics2D.CircleCast(rb.position -0.35f * Vector2.right, 0.35f, Vector2.down, 0.1f, groundMask);

        return hit.collider != null || hit2.collider != null;
    }

    public override void OnStun()
    {
        jumpCharging = false; // Cancel jump
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawLine(rb.position - 0.25f * Vector2.up - jumpChargeDistance * Vector2.right, rb.position - 0.25f * Vector2.up + jumpChargeDistance * Vector2.right);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(rb.position + 0.35f * Vector2.right, 0.35f);
        Gizmos.DrawWireSphere(rb.position - 0.35f * Vector2.right, 0.35f);
        Gizmos.DrawWireSphere(rb.position + 0.35f * Vector2.right + 0.1f * Vector2.down, 0.35f);
        Gizmos.DrawWireSphere(rb.position - 0.35f * Vector2.right + 0.1f * Vector2.down, 0.35f);
    }
}
