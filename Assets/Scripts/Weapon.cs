using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [SerializeField] protected Transform shotPoint = default;

    [SerializeField] protected float timeBetweenShots = 1f;
    [Tooltip("Random.Range(-accuracyOffset, +accuracyOffset) is added to rotation of projectile on click. For reference, 0 equals perfect accuracy, while 5 makes you miss slightly; 25 makes it a cone, basically")]
    [SerializeField] protected float accuracyOffset = 0f; //added to rotation; accuracyOffset of 0 == perfect accuracy
    [SerializeField] protected int damage = 1;

    [Header("Ammo related variables")]

    [SerializeField] protected WeaponType myWeaponType = WeaponType.Infinite;
    [SerializeField] protected int magazineSize = 6;
    [SerializeField] protected float reloadSpeed = 2f;
    [SerializeField] protected int ammoPerShot = 1;

    protected int currentLoadedAmmo;
    protected bool isReloading = false;
    protected bool isOnCooldown = false; 
    protected WeaponManager weaponManager;

    public virtual void Start()
    {
        weaponManager = WeaponManager.Instance;
        currentLoadedAmmo = magazineSize;
    }

    public virtual void Update()
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

    protected bool CanShoot()
    {
        bool shotPointIsBlocked = false;
        //checks if weapon's shot point is not inside a wall
        Collider2D col = Physics2D.OverlapCircle(shotPoint.position, .1f);
        if (col != null)
        {
            shotPointIsBlocked = col.tag == "Obstacle";
        }
        //Debug.Log("IS SHOTPOINT BLOCKED? " + shotPointIsBlocked);
        return !isOnCooldown && currentLoadedAmmo > 0 && !isReloading && !shotPointIsBlocked;
    }

    public virtual void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
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

        //Debug.Log("CURRENT AMMO: " + currentAmmo);
    }

    /// <summary>
    /// ShootCoroutine is always called for all weapon scripts, must be overwritten for each weapon type.
    /// On starting, isOnCooldown is set to true and currentLoadedAmmo is already subtracted.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator ShootCoroutine()
    {
        yield return new WaitForSeconds(timeBetweenShots);
    }

    protected void Reload()
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

    protected IEnumerator ReloadRoutine()
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
