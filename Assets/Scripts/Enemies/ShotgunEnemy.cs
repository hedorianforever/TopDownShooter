using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunEnemy : GunmanEnemy
{
    //[SerializeField] protected AudioClip shootSound;
    [SerializeField] Transform[] shotPoints;

    public override IEnumerator ShootRoutine()
    {
        //AudioManager.Instance.PlayClip(shootSound, .6f);
        foreach (Transform shotPoint in shotPoints)
        {
            GameObject projectile = Instantiate(projectilePrefab, shotPoint.position, shotPoint.rotation) as GameObject;

            if (projectile != null)
            {
                projectile.GetComponent<EnemyProjectile>().Init(attackDamage, projectileSpeed);
            }
        }

        yield return new WaitForSeconds(timeBetweenAttacks);

        isShooting = false;
    }
}
