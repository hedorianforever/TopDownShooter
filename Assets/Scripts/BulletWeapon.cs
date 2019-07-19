using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletWeapon : Weapon
{
    [Header("BulletWeapon specific variables")]
    [SerializeField] protected GameObject projectilePrefab = default;
    [SerializeField] float projectileSpeed = 11f;
    [SerializeField] float projectileLifetime = 1f;

    public override IEnumerator ShootCoroutine()
    {
        GameObject projectile = Instantiate(projectilePrefab, shotPoint.position, transform.rotation) as GameObject;

        float randomRotation = Random.Range(-accuracyOffset, accuracyOffset);
        projectile.transform.Rotate(new Vector3(
            projectile.transform.rotation.x,
            projectile.transform.rotation.y,
            projectile.transform.rotation.z + randomRotation)
        );

        projectile.GetComponent<Projectile>().Init(projectileSpeed, projectileLifetime, damage);

        yield return new WaitForSeconds(timeBetweenShots);

        isOnCooldown = false;
    }
}
