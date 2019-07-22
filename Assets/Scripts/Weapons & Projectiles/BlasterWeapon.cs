using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterWeapon : Weapon
{
    [Header("BlasterWeapon specific variables")]
    [SerializeField] protected GameObject triggerProjectilePrefab = default;

    public override IEnumerator ShootCoroutine()
    {
        GameObject projectile = Instantiate(triggerProjectilePrefab, shotPoint.position, transform.rotation) as GameObject;

        float randomRotation = Random.Range(-accuracyOffset, accuracyOffset);
        projectile.transform.Rotate(new Vector3(
            projectile.transform.rotation.x,
            projectile.transform.rotation.y,
            projectile.transform.rotation.z + randomRotation)
        );

        projectile.GetComponent<TriggerProjectile>().Init(damage);
        projectile.transform.SetParent(shotPoint, true);

        yield return null;
    }
}
