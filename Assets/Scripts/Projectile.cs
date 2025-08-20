using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;

    [Space]

    public ParticleSystem deathEffect;

    [Space]

    [SerializeField] private float damage;
    [SerializeField] private float lifetime;
    private float lifeTimer;

    private bool dead;

    private void Start()
    {
        lifeTimer = lifetime;
    }

    private void Update()
    {
        lifeTimer = Mathf.Max(lifeTimer - Time.deltaTime, 0.0f);

        if (lifeTimer == 0.0f) Die();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Health.TryDamage(collision.gameObject, damage, gameObject);

        Die();
    }

    public void Die()
    {
        if (dead) return;

        Instantiate(deathEffect, transform.position, transform.rotation);

        Destroy(gameObject);

        dead = true;
    }
}
