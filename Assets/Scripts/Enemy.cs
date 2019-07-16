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

    [HideInInspector] public Transform playerTransform;
    protected bool hasNoticedPlayer = false;
    protected Animator anim;

    public virtual void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }

    public virtual void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        hasNoticedPlayer = true;

        if (health <= 0)
        {
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
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, noticePlayerRadius);
    }
}
