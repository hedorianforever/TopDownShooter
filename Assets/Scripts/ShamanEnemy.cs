using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ShamanEnemy : Enemy
{
    [SerializeField] float timeToSummon = 2f;
    [SerializeField] GameObject minionPrefab = default;
    [SerializeField] Transform[] destinations = default;

    private AIPath aiPath;
    private AIDestinationSetter aiDestSetter;

    private bool isSummoning = false;

    public override void Start()
    {
        base.Start();
        aiPath = GetComponent<AIPath>();
        aiDestSetter = GetComponent<AIDestinationSetter>();

        aiDestSetter.target = GetRandomDestination();
        anim.SetBool("isMoving", true);

        aiPath.maxSpeed = moveSpeed;
    }

    private void Update()
    {
        if (playerTransform == null) { return; }

        if (aiPath.velocity.x > 1f || aiPath.velocity.y > 1f)
        {
            anim.SetBool("isMoving", true);
        }

        if (Vector2.Distance(transform.position, (Vector2)aiDestSetter.target.position) < aiPath.endReachedDistance)
        {
            anim.SetBool("isMoving", false);
            CheckForPlayer();
            if (hasNoticedPlayer && !isSummoning)
            {
                FacePlayer();
                isSummoning = true;
                StartCoroutine(SummonRoutine());
            }
        }
    }

    private Transform GetRandomDestination()
    {
        return destinations[Random.Range(0, destinations.Length)];
    }

    /// <summary>
    /// Returns true if player is near the destination
    /// </summary>
    private bool IsPlayerNear(Transform destination)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(destination.position, noticePlayerRadius);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == "Player")
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator SummonRoutine()
    {
        //summon enemy
        anim.SetTrigger("summonTrigger");
        yield return new WaitForSeconds(timeToSummon);

        Transform newDestination = GetRandomDestination();
        while (newDestination == aiDestSetter.target || IsPlayerNear(newDestination))
        {
            newDestination = GetRandomDestination();
            yield return null;
        }
        aiDestSetter.target = newDestination;
        hasNoticedPlayer = false;
        isSummoning = false;
    }

    /// <summary>
    /// called from the animator to summon the minion and resume the routine
    /// </summary>
    public void SummonMinion()
    {
        //instantiate summon effect
        Instantiate(minionPrefab, transform.position + new Vector3(-1, 0, 0), Quaternion.identity);
    }
}
