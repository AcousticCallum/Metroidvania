using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [Space]

    public SpriteRenderer rend;

    [Space]

    public float detectionRadius;
    public float damage;
    public float stunDuration;
    public float groundAngle;
    public LayerMask groundMask;

    protected Vector2 toPlayer;
    protected float stunTimer;
    protected bool chasing;

    private void Update()
    {
        stunTimer = Mathf.Max(stunTimer - Time.deltaTime, 0.0f);
        if (stunTimer > 0.0f) return;

        if (Detect())
        {
            DetectMove();
            chasing = true;
            return;
        }

        DefaultMove();
    }

    private void LateUpdate()
    {
        UpdateAnims();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Player.instance.gameObject)
        {
            stunTimer = stunDuration;
            Health.Damage(collision.gameObject, damage);
            return;
        }

        // collision.gameObject.layer is in groundMask
        if (groundMask == (groundMask | (1 << collision.gameObject.layer)))
        {
            if (Vector2.Angle(collision.GetContact(0).normal, Vector2.up) > groundAngle)
            {
                stunTimer = stunDuration;
            }
        }
    }

    public override void OnDamage()
    {
        chasing = true;
    }

    public override void OnDie()
    {

    }

    protected virtual bool Detect()
    {
        toPlayer = Player.instance.GetCenter() - (Vector2)transform.position;

        // Override many checks if chasing
        if (chasing) return true;

        if (toPlayer.magnitude > detectionRadius) return false;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer.normalized, toPlayer.magnitude);
        if (!hit) return false;
        if (hit.collider.gameObject != Player.instance.gameObject) return false;

        return true;
    }

    protected virtual void DetectMove()
    {
        
    }

    protected virtual void DefaultMove()
    {

    }

    protected virtual void UpdateAnims()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
