using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] GameObject destroyFX = default;

    private float lifetime;
    private float speed;
    private int damage;

    public void Init(int enemyDamage, float enemySpeed, float enemyLifetime)
    {
        lifetime = enemyLifetime;
        speed = enemySpeed;
        damage = enemyDamage;
    }

    private void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.GetComponent<Player>().TakeDamage(damage);
            DestroyProjectile();
        }
        else if (collision.tag == "Obstacle")
        {
            DestroyProjectile();
        }
    }




}
