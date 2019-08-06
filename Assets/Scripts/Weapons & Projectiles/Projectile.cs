using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject destroyVFX = default;
    [SerializeField] GameObject destroyByTimeVFX = default;
    [SerializeField] AudioClip impactSFX = default;
    [Range(0, 1)] [SerializeField] float impactSFXVolume = .4f;

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
        if (destroyVFX != null)
        {
            Instantiate(destroyVFX, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("destroyFX missing at projectile " + name);
        }
        Destroy(gameObject);
    }

    private void DestroyProjectileByTime()
    {
        if (destroyByTimeVFX != null)
        {
            Instantiate(destroyByTimeVFX, transform.position, Quaternion.identity);
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
            if (collision.GetComponent<Enemy>() != null)
            {
                collision.GetComponent<Enemy>().TakeDamage(damage);
                AudioManager.Instance.PlayClip(impactSFX, impactSFXVolume);
            }
            DestroyProjectile();
        }
        else if (collision.tag == "Obstacle")
        {
            DestroyProjectile();
        }
    }

}
