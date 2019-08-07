using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ShamanEnemy : WandererEnemy
{
    [Header("Shaman related variables")]
    [SerializeField] float timeToSummon = 2f;
    [SerializeField] GameObject minionPrefab = default;
    [SerializeField] float timeBetweenSummons = 2.5f;
    [SerializeField] GameObject enemyPortal = default;

    private bool isOnSummonCooldown = false;
    private bool isSummoning = false;

    public override void Start()
    {
        base.Start();
        //aiPath = GetComponent<AIPath>();
        //aiDestSetter = GetComponent<AIDestinationSetter>();

        //aiDestSetter.target = GetRandomDestination();
        //anim.SetBool("isMoving", true);

        //aiPath.maxSpeed = moveSpeed;
    }

    public override void Update()
    {
        base.Update();

        if ((ai.reachedEndOfPath || !ai.hasPath) && !ai.pathPending && !isSummoning && !anim.GetBool("isMoving") && !isOnSummonCooldown)
        {
            FacePlayer();
            anim.SetBool("isMoving", false);
            isSummoning = true;
            isOnSummonCooldown = true;
            StartCoroutine(SummonRoutine());
        }

    }

    IEnumerator SummonRoutine()
    {
        //summon enemy
        anim.SetTrigger("summonTrigger");

        yield return new WaitForSeconds(timeToSummon);

        yield return new WaitForSeconds(timeBetweenSummons);

        isOnSummonCooldown = false;
    }

    /// <summary>
    /// called from the animator to summon the minion and resume the routine
    /// </summary>
    public void SummonMinion()
    {
        //instantiate summon effect
        Vector3 summonPos = GetValidSummonPosition();
        GameObject portal = Instantiate(enemyPortal, summonPos, Quaternion.identity);
        portal.GetComponent<SummonPortal>().SetEnemyToSummon(minionPrefab);
        isSummoning = false;

    }

    //returns a position near the shaman which is not occupied by an obstacle
    private Vector3 GetValidSummonPosition()
    {
        Collider2D collider;

        Vector3 summonPos = transform.position + new Vector3(1, 0, 0);
        collider = Physics2D.OverlapCircle(summonPos, .2f, 1 << LayerMask.NameToLayer("Obstacle"));

        if (collider == null)
        {
            return summonPos;
        }

        summonPos = transform.position + new Vector3(-1, 0, 0);
        collider = Physics2D.OverlapCircle(summonPos, .2f, 1 << LayerMask.NameToLayer("Obstacle"));

        if (collider == null)
        {
            return summonPos;
        }

        summonPos = transform.position + new Vector3(0, 1, 0);
        collider = Physics2D.OverlapCircle(summonPos, .2f, 1 << LayerMask.NameToLayer("Obstacle"));

        if (collider == null)
        {
            return summonPos;
        }

        summonPos = transform.position + new Vector3(0, -1, 0);
        collider = Physics2D.OverlapCircle(summonPos, .2f, 1 << LayerMask.NameToLayer("Obstacle"));

        if (collider == null)
        {
            return summonPos;
        }

        return transform.position;
    }
}
