using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    [SerializeField] protected int health;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float timeBetweenAttacks;
    [SerializeField] protected int attackDamage;
    [SerializeField] protected float noticePlayerRadius = 15f;
    [SerializeField] protected GameObject deathVFX;


    [HideInInspector] public Transform playerTransform;

    [Header("Drops related variables")]
    [SerializeField] protected AmmoPickup ammoDrop;
    [SerializeField] protected HealthPickup healthDrop;
    [Range(0, 1)] [SerializeField] protected float ammoDropChance;
    [Range(0, 1)] [SerializeField] protected float healthDropChance;

    protected bool hasNoticedPlayer = false;
    protected Animator anim;
    protected bool isAlive = true;

    public virtual void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }

    public virtual void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        hasNoticedPlayer = true;

        //isAlive bool is needed or else Die() is called multiple times if the enemy is hit (and killed) by more than 1 bullet
        if (health <= 0 && isAlive)
        {
            isAlive = false;
            Die();
        }
    }

    protected void CheckForPlayer()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, noticePlayerRadius);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == "Player")
            {
                hasNoticedPlayer = true;
                return;
            }
        }
    }

    protected void FacePlayer()
    {
        if (playerTransform.position.x - transform.position.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (playerTransform.position.x - transform.position.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    void Die()
    {
        if (Random.Range(0, .9999f) < healthDropChance)
        {
            Instantiate(healthDrop, transform.position, Quaternion.identity);
        }
        if (Random.Range(0, .9999f) < ammoDropChance)
        {
            Instantiate(ammoDrop, transform.position, Quaternion.identity);
        }
        GameManager.Instance.DecreaseEnemyCount(transform.position);

        if (deathVFX != null)
        {
            Instantiate(deathVFX, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, noticePlayerRadius);
    }
}
