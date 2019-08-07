using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MeleeEnemy : Enemy
{
    [SerializeField] float stopDistance = 3f;
    [SerializeField] float leapAttackSpeed = 4f;

    private AIPath aiPath;

    private bool isAttacking = false;

    public override void Start()
    {
        base.Start();
        aiPath = GetComponent<AIPath>();

        aiPath.maxSpeed = moveSpeed;
        aiPath.endReachedDistance = stopDistance;
        aiPath.canMove = false;

        if (playerTransform != null && GetComponent<AIDestinationSetter>().target == null)
        {
            GetComponent<AIDestinationSetter>().target = playerTransform;
        }
    }

    private void Update()
    {
        if (playerTransform == null) { return; }

        if ((Mathf.Abs(aiPath.velocity.y) >= .1f || Mathf.Abs(aiPath.velocity.x) >= .1f) && !isAttacking)
        {
            anim.SetBool("isMoving", true);

        }
        else
        {
            anim.SetBool("isMoving", false);
        }

        if (!hasNoticedPlayer)
        {
            CheckForPlayer();
            return;
        }

        if (hasNoticedPlayer && !isAttacking)
        {
            aiPath.canMove = true;
        }

        FacePlayer();

        if (Vector2.Distance(transform.position, playerTransform.position) <= stopDistance && !isAttacking)
        {
            Attack();
        }

    }

    private void Attack()
    {
        aiPath.canMove = false;
        isAttacking = true;

        anim.SetBool("isMoving", false);

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        Vector2 originalPos = transform.position;
        Vector2 targetPos = playerTransform.position;

        float percent = 0;
        bool hasDamaged = false;

        //leap attack
        while (percent <= 1)
        {
            percent += Time.deltaTime * leapAttackSpeed;

            //function where y is 0 when x is 0, y is 1 when x is .5, y is 0 when x is 1
            float formula = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector2.Lerp(originalPos, targetPos, formula);

            //damage when touching the player, probably should update to use physics (so the player can dodge)
            if (!hasDamaged && Vector2.Distance(transform.position, playerTransform.position) < 1f)
            {
                // TODO : slashfx
                if (playerTransform != null)
                {
                    if (playerTransform.GetComponent<Player>() != null)
                    {
                        //Debug.Log("ATTACK DAMAGE OF " + name + ": " + attackDamage);
                        playerTransform.GetComponent<Player>().TakeDamage(attackDamage);
                        hasDamaged = true;
                    }
                }
            }
            yield return null;
        }

        //wait for cooldown between attacks(heavy breathing, for example) to resume going after player again
        yield return new WaitForSeconds(timeBetweenAttacks);
        aiPath.canMove = true;
        anim.SetBool("isMoving", true);
        isAttacking = false;
    }

    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);
        if (!hasNoticedPlayer)
        {
            hasNoticedPlayer = true;
            aiPath.canMove = true;
        }
    }
}

