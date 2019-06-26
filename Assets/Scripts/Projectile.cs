using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
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
        }
        else
        {
            Debug.Log("destroyFX missing at projectile " + name);
        }
        Destroy(gameObject);
    }

    public void Init(Weapon projectileWeapon)
    {
        weapon = projectileWeapon;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            collision.GetComponent<Enemy>().TakeDamage(weapon.GetDamage());
            DestroyProjectile();
        }
        else if (collision.tag == "Obstacle")
        {
            DestroyProjectile();
        }
    }

}
