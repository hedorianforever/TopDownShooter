using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject destroyFX = default;
    [SerializeField] GameObject destroyByTimeFX = default;

    private float speed = 0;
    private int damage = 0;

    private BulletWeapon weapon;

    private void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    public void Init(float projectileSpeed, float projectileLifetime, int projectileDamage)
    {
        speed = projectileSpeed;
        damage = projectileDamage;
        Invoke("DestroyProjectileByTime", projectileLifetime);
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

    private void DestroyProjectileByTime()
    {
        if (destroyByTimeFX != null)
        {
            Instantiate(destroyByTimeFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        } else
        {
            //use normal destroy fx
            DestroyProjectile();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            collision.GetComponent<Enemy>().TakeDamage(damage);
            DestroyProjectile();
        }
        else if (collision.tag == "Obstacle")
        {
            DestroyProjectile();
        }
    }

}
