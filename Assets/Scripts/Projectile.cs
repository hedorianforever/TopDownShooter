using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject destroyFX = default;

    private Weapon weapon;

    private void Start()
    {
        Invoke("DestroyProjectile", weapon.projectileLifetime);
    }

    private void Update()
    {
        if (weapon == null)
        {
            return;
        }

        transform.Translate(Vector2.right * weapon.projectileSpeed * Time.deltaTime);
    }

    private void DestroyProjectile()
    {
        if (destroyFX != null)
        {
            Instantiate(destroyFX, transform.position, Quaternion.identity);
        } else
        {
            Debug.Log("destroyFX missing at projectile " + name);
        }
        Destroy(gameObject);
    }

    public void Init(Weapon projectileWeapon)
    {
        weapon = projectileWeapon;
    }

}
