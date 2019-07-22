using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] GameObject destroyFX = default;

    private float lifetime;
    private float speed;
    private int damage;
    Quaternion rotationToMove;

    private void Start()
    {
        rotationToMove = transform.rotation;
    }

    public void Init(int enemyDamage, float enemySpeed, float enemyLifetime)
    {
        lifetime = enemyLifetime;
        speed = enemySpeed;
        damage = enemyDamage;
    }

    private void Update()
    {
        //this rotation changing is done so the projectile will move in the right direction but will appear to the player as if it has no rotation
        transform.rotation = rotationToMove;
        transform.Translate(Vector2.right * speed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, 0);
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
        //Debug.Log("ENEMY PROJECTILE " + name + " COLLIDED WITH " + collision.name + " WITH TAG " + collision.tag);
        if (collision.tag == "Player")
        {
            if (collision.GetComponent<Player>().GetIsInvulnerable())
            {
                return;
            }
            collision.GetComponent<Player>().TakeDamage(damage);
            DestroyProjectile();
        }
        else if (collision.tag == "Obstacle")
        {
            DestroyProjectile();
        }
    }




}
