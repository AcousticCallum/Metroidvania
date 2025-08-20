using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;
using static UnityEngine.EventSystems.EventTrigger;

public class Health : MonoBehaviour
{
    private const float deathHeight = -25.0f;

    public ParticleSystem deathEffect;

    [Space]

    [SerializeField] private Renderer[] rends;
    [SerializeField] private float damageFlashDuration;
    private float damageFlashTimer;
    private bool damageFlashOngoing;

    [Space]

    public float maxHealth;

    [Space]

    public float invincibilityDuration;
    private float invincibilityTimer;
    private bool invincible;

    [Space]

    public LayerMask invulnerableMask;

    [Space]

    public bool disableOnDeath;

    [Space]

    public string damageSound;
    public string dieSound;

    private float health;
    private bool dead;

    private void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        if (transform.position.y <= deathHeight)
        {
            Die();
        }

        damageFlashTimer = Mathf.Max(damageFlashTimer - Time.deltaTime, 0.0f);
        if (damageFlashTimer == 0.0f && damageFlashOngoing) ToggleDamageFlash(false);

        invincibilityTimer = Mathf.Max(invincibilityTimer - Time.deltaTime, 0.0f);
        invincible = invincibilityTimer > 0.0f;
    }

    public static bool TryDamage(GameObject target, float amount = 1.0f, GameObject source = null)
    {
        if (target.TryGetComponent(out Health health))
        {
            if(health.dead) return false;

            // Source is in mask
            if (health.invulnerableMask == (health.invulnerableMask | (1 << source.layer))) return false;

            health.Damage(amount);

            return true;
        }

        return false;
    }

    public static bool IsDead(GameObject target)
    {
        if (target.TryGetComponent(out Health health))
        {
            return health.dead;
        }

        return false;
    }

    public void Damage(float amount = 1.0f)
    {
        if (dead) return;
        if (invincible) return;

        health = Mathf.Max(health - amount, 0.0f);
        if (health == 0.0f) Die();

        if(TryGetComponent(out Entity entity)) entity.OnDamage();

        damageFlashTimer = damageFlashDuration;
        if (damageFlashTimer > 0.0f && !damageFlashOngoing) ToggleDamageFlash(true);

        invincibilityTimer = invincibilityDuration;
        invincible = invincibilityTimer > 0.0f;

        SoundManager.instance.PlaySound(damageSound);
    }

    public void Die()
    {
        if (dead) return;

        if (TryGetComponent(out Entity entity)) entity.OnDie();

        Instantiate(deathEffect, transform.position, transform.rotation);
        
        if(disableOnDeath) gameObject.SetActive(false);
        else Destroy(gameObject);

        dead = true;

        SoundManager.instance.PlaySound(dieSound);
    }

    private void ToggleDamageFlash(bool toggleOn)
    {
        foreach (Renderer rend in rends)
        {
            Material newMat = rend.material;
            newMat.SetFloat("_Damage_Blend", toggleOn ? 0.5f : 0.0f);
            rend.material = newMat;
        }

        damageFlashOngoing = toggleOn;
    }

    public float GetHealthRatio()
    {
        return health / maxHealth;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(deathHeight * Vector3.up + -50.0f * Vector3.right, deathHeight * Vector3.up + 50.0f * Vector3.right);
    }
}
