using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class AmmoPickup : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            WeaponManager weaponManager = WeaponManager.Instance;
            WeaponType currentWeapon = weaponManager.GetEquippedWeaponType();

            if (weaponManager.GetCurrentAmmo(currentWeapon) == weaponManager.GetMaxAmmo(currentWeapon))
            {
                //FULL AMMO
                //show some kind of message
            }
            else
            {
                //adds half the max ammo of the equipped weapon's type 
                weaponManager.AddAmmo(currentWeapon, weaponManager.GetMaxAmmo(currentWeapon) / 2);
                //show something like + 100 (currentWeapon.name) ammo
                Destroy(gameObject);
            }
        }
    }
}
