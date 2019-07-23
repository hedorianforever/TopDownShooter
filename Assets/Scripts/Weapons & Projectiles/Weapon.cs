using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Weapon : MonoBehaviour
{

    [SerializeField] protected Transform shotPoint = default;
    [SerializeField] protected AudioClip shootingSound = default;

    [SerializeField] protected float timeBetweenShots = 1f;
    [Tooltip("Random.Range(-accuracyOffset, +accuracyOffset) is added to rotation of projectile on click. For reference, 0 equals perfect accuracy, while 5 makes you miss slightly; 25 makes it a cone, basically")]
    [SerializeField] protected float accuracyOffset = 0f; //added to rotation; accuracyOffset of 0 == perfect accuracy
    [SerializeField] protected int damage = 1;

    [Header("Ammo related variables")]

    [SerializeField] protected WeaponType myWeaponType = WeaponType.Infinite;
    [SerializeField] protected int ammoPerShot = 1;

    public WeaponType MyWeaponType
    {
        get
        {
            return myWeaponType;
        }
    }

    //protected bool isOnCooldown = false; 
    protected WeaponManager weaponManager;

    public virtual void Start()
    {
        weaponManager = WeaponManager.Instance;
    }

    public virtual void Update()
    {
        LookAtMouse();
        TryShoot();
    }

    private void LookAtMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDirection = mousePos - transform.position;

        lookDirection.Normalize(); //not really necessary, just showing that it doesn't matter

        transform.up = lookDirection; //transform.up = basically the same as rotation.z

        transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z + 90)); //add 90 to final rotation because the weapon is initially looking right;
    }

    //virtual so weapons that don't care about hitting obstacles can override it
    protected virtual bool CanShoot()
    {
        bool shotPointIsBlocked = false;
        //checks if weapon's shot point is not inside a wall
        Collider2D col = Physics2D.OverlapCircle(shotPoint.position, .1f, 1 << LayerMask.NameToLayer("Obstacle"));
        if (col != null)
        {
            Debug.Log(col.name);
            shotPointIsBlocked = col.tag == "Obstacle";
        }
        return !weaponManager.IsOnCooldown() && !shotPointIsBlocked && (weaponManager.GetCurrentAmmo(myWeaponType) - ammoPerShot >= 0);
    }

    public virtual void TryShoot()
    {
        if (Input.GetMouseButtonDown(0) && CanShoot())
        {
            Shoot();
        }
    }

    public virtual void Shoot()
    {
        CameraShaker.Instance.ShakeOnce(1.5f, 3f, .05f, .15f);
        AudioManager.Instance.PlayClip(shootingSound);

        if (myWeaponType == WeaponType.Infinite)
        {
            weaponManager.SetCooldown(timeBetweenShots);
            StartCoroutine(ShootCoroutine());
        } else
        {
            weaponManager.UseAmmo(myWeaponType, ammoPerShot);
            weaponManager.SetCooldown(timeBetweenShots);
            StartCoroutine(ShootCoroutine());
        }
    }

    /// <summary>
    /// ShootCoroutine is always called for all weapon scripts, must be overwritten for each weapon type.
    /// On starting, isOnCooldown is set to true and currentLoadedAmmo is already subtracted.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator ShootCoroutine()
    {
        yield return null;
    }

    public int GetDamage()
    {
        return damage;
    }
}
