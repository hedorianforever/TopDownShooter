using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunWeapon : BulletWeapon
{
    public override void TryShoot()
    {
        //instead of original shoot function, can keep the button pressed
        if (Input.GetMouseButton(0) && CanShoot())
        {
            Shoot();
        }
    }
}
