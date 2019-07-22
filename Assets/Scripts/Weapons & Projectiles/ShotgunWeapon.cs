using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunWeapon : BulletWeapon
{
    [Tooltip("Shotgun specific variables")]
    [SerializeField] int numberOfBullets = 5;
    //this variation will make so that some bullets travel further than others
    [SerializeField] float speedVariationPerBullet = 1.5f;

    public override IEnumerator ShootCoroutine()
    {
        int damagePerBullet = damage / numberOfBullets;
        for (int i = 0; i < numberOfBullets; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab, shotPoint.position, transform.rotation) as GameObject;

            float randomRotation = Random.Range(-accuracyOffset, accuracyOffset);
            projectile.transform.Rotate(new Vector3(
                projectile.transform.rotation.x,
                projectile.transform.rotation.y,
                projectile.transform.rotation.z + randomRotation)
            );

            projectile.GetComponent<Projectile>().Init(projectileSpeed + Random.Range(-speedVariationPerBullet, speedVariationPerBullet), projectileLifetime, damagePerBullet);
        }

        yield return new WaitForSeconds(timeBetweenShots);
    }
}
