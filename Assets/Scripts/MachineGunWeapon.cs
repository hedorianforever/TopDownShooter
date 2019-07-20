using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunWeapon : BulletWeapon
{
    public override void Shoot()
    {
        //instead of original shoot function, can keep the button pressed
        if (Input.GetMouseButton(0))
        {
            if (CanShoot())
            {
                currentLoadedAmmo -= ammoPerShot;
                isOnCooldown = true;
                StartCoroutine(ShootCoroutine());
            }
        }

        if (Input.GetKeyDown(KeyCode.R) || (Input.GetMouseButtonDown(0) && currentLoadedAmmo <= 0))
        {
            Reload();
        }
    }
}
