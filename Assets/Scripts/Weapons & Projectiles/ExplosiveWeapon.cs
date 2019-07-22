using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveWeapon : Weapon
{
    [Header("ExplosiveWeapon specific variables")]
    [SerializeField] protected Explosive explosivePrefab = default;
    [SerializeField] protected float launchSpeed = 5f;

    public override IEnumerator ShootCoroutine()
    {
        Explosive explosive = Instantiate(explosivePrefab, shotPoint.position, Quaternion.identity);

        float randomRotation = Random.Range(-accuracyOffset, accuracyOffset);
        explosive.transform.Rotate(new Vector3(
            explosive.transform.rotation.x,
            explosive.transform.rotation.y,
            explosive.transform.rotation.z + randomRotation)
        );

        explosive.LaunchAtDirection(shotPoint.right, launchSpeed, damage);

        yield return null;
    }
}
