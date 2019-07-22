using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Explosive : MonoBehaviour
{
    [SerializeField] float explosionRadius = 1f;
    [SerializeField] float timeToExplode = 1f;

    private int damage;

    [SerializeField] GameObject explosionVFX = default;

    void Start()
    {
        Invoke("Explode", timeToExplode);
    }

    void Explode()
    {
        if (explosionVFX == null)
        {
            Debug.Log("NO EXPLOSION VFX AT " + name);
        } else
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }

        //deal damage to everyone in the radius
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach(Collider2D col in hitColliders)
        {
            if (col.tag == "Player")
            {
                //all explosives deal the same damage to the player
                col.GetComponent<Player>().TakeDamage(5);
                //Debug.Log(col.name + " has taken " + damage + " damage from " + name);
            } else if (col.tag == "Enemy")
            {
                col.GetComponent<Enemy>().TakeDamage(damage);
                //Debug.Log(col.name + " has taken " + damage + " damage from " + name);
            }
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Explode();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    public void LaunchAtDirection(Vector3 direction, float launchSpeed, int weaponDamage)
    {
        damage = weaponDamage;
        GetComponent<Rigidbody2D>().AddForce(direction * launchSpeed);
    }
}
