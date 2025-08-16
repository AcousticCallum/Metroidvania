using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Animations.Rigging;

public class PlayerWeapon : MonoBehaviour
{
    private bool activated;

    [Space]

    public Transform weaponEnd;
    public Transform aimTarget;
    public MultiPositionConstraint multiPositionConstraint;
    private Vector2 aimDir;

    [Space]

    [SerializeField] private float shootCooldown;
    private float shootTimer;

    [SerializeField] private float shootSpread;
    [SerializeField] private int shootCount;

    [Space]

    public Projectile projectilePrefab;
    [SerializeField] float projectileSpeed;

    private void Update()
    {
        Vector3 mouseWorldPos = MainCamera.instance.mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0.0f;

        aimTarget.position = mouseWorldPos;

        Vector2 toAimTarget = (Vector2)aimTarget.position - Player.instance.GetCenter();
        if(toAimTarget.sqrMagnitude < 1.0f) aimTarget.position = Player.instance.GetCenter() + toAimTarget.normalized;

        Vector3 weaponEndPos = weaponEnd.position;
        weaponEndPos.z = 0.0f;
        aimDir = (aimTarget.position - weaponEndPos).normalized;

        shootTimer = Mathf.Max(shootTimer - Time.deltaTime, 0.0f);

        if(activated && shootTimer == 0.0f)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        // Repeat for all shots
        for (int i = 0; i < shootCount; i++)
        {
            // Pos
            Vector3 projectilePos = weaponEnd.position;
            projectilePos.z = 0.0f;

            // Rot
            float rotationAngle = Vector2.SignedAngle(Vector2.right, aimDir);
            float randomAngle = Random.Range(-shootSpread * 0.5f, shootSpread * 0.5f);
            Quaternion projectileRot = Quaternion.Euler((rotationAngle + randomAngle) * Vector3.forward);

            // Instantiate
            Projectile projectile = Instantiate(projectilePrefab, projectilePos, projectileRot);

            // Speed
            projectile.rb.velocity = projectileSpeed * projectile.transform.right;
        }

        // Misc
        SoundManager.instance.PlaySound("Shoot");

        shootTimer = shootCooldown;
    }

    public void Activate()
    {
        activated = true;
    }

    public void Deactivate()
    {
        activated = false;
    }
}
