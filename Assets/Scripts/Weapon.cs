using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab = default;
    [SerializeField] Transform shotPoint = default;
    [SerializeField] float timeBetweenShots = 1f;
    [Tooltip("Random.Range(-accuracyOffset, +accuracyOffset) is added to rotation of projectile on click. For reference, 0 equals perfect accuracy, while 5 makes you miss slightly; 25 makes it a cone, basically")]
    [SerializeField] float accuracyOffset = 0f; //added to rotation; accuracyOffset of 0 == perfect accuracy
    [SerializeField] int damage = 1;

    public float projectileSpeed = 10f;
    public float projectileLifetime = 5f;

    [Header("Ammo related variables")]

    [SerializeField] WeaponType myWeaponType = WeaponType.Infinite;
    [SerializeField] int magazineSize = 6;
    [SerializeField] float reloadSpeed = 2f;
    [SerializeField] int ammoPerShot = 1;

    private int currentLoadedAmmo = 0;
    private bool isReloading = false;
    private bool isOnCooldown = false;
    private WeaponManager weaponManager;

    private void Start()
    {
        weaponManager = WeaponManager.Instance;
        weaponManager.AddAmmo(myWeaponType, magazineSize);
        float originalReloadSpeed = reloadSpeed;
        reloadSpeed = 0f;
        Reload();
        reloadSpeed = originalReloadSpeed;
    }
    private void Update()
    {
        LookAtMouse();
        Shoot();

        //if (isReloading)
        //{
        //    Debug.Log("RELOADING...");
        //}
    }

    private void LookAtMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDirection = mousePos - transform.position;

        lookDirection.Normalize(); //not really necessary, just showing that it doesn't matter

        transform.up = lookDirection; //transform.up = basically the same as rotation.z

        transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z + 90)); //add 90 to final rotation because the weapon is initially looking right;
    }

    private void Shoot()
    {
        if (Input.GetMouseButton(0) )
        {
            if (!isOnCooldown && currentLoadedAmmo > 0 && !isReloading)
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

        //Debug.Log("CURRENT AMMO: " + currentAmmo);
    }

    IEnumerator ShootCoroutine()
    {
        GameObject projectile = Instantiate(projectilePrefab, shotPoint.position, transform.rotation) as GameObject;

        float randomRotation = Random.Range(-accuracyOffset, accuracyOffset);
        projectile.transform.Rotate(new Vector3(
            projectile.transform.rotation.x,
            projectile.transform.rotation.y, 
            projectile.transform.rotation.z + randomRotation)
        );

        projectile.GetComponent<Projectile>().Init(this);

        yield return new WaitForSeconds(timeBetweenShots);

        isOnCooldown = false;
    }

    private void Reload()
    {
        if (weaponManager.GetCurrentAmmo(myWeaponType) <= 0)
        {
            //play error sound / show not allowed sign
            Debug.Log("NO AMMO! ");
            return;
        }

        if (currentLoadedAmmo == magazineSize || isReloading)
        {
            //Debug.Log("WEAPON FULLY LOADED, CANT RELOAD");
            return;
        }

        StartCoroutine(ReloadRoutine());
    }

    IEnumerator ReloadRoutine()
    {
        isReloading = true;
        //reload animation
        yield return new WaitForSeconds(reloadSpeed);
        if (myWeaponType == WeaponType.Infinite)
        {
            currentLoadedAmmo = magazineSize;
        } else
        {
            int currentAmmo = weaponManager.GetCurrentAmmo(myWeaponType);
            int initialAmmo = currentAmmo;
            while (currentAmmo != 0 && currentLoadedAmmo != magazineSize)
            {
                currentLoadedAmmo++;
                currentAmmo--;
                yield return null;
            }
            weaponManager.UseAmmo(myWeaponType, initialAmmo - currentAmmo);
        }
        isReloading = false;
    }

    public int GetDamage()
    {
        return damage;
    }
}
