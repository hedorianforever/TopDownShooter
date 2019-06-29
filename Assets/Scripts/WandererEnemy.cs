using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class WandererEnemy : Enemy
{
    [SerializeField] float wanderRadius = 10f;
    [SerializeField] float timeBetweenWanderingMax = 10f;
    [SerializeField] float timeBetweenWanderingMin = 1.5f;
    [Tooltip("Minimum distance the next wander point must be from the current position")]
    [SerializeField] float minWanderDistance = 2f;
    [Tooltip("Maximum time the enemy should move. Used so that if he picks a near position that's a LONG walk away (as in there are a lot of obstacles in the way) he will stop moving.")]
    [SerializeField] float maxTimeWalking = 3f;

    private AIPath aiPath;
    private IAstarAI ai;

    private bool isOnWanderingCooldown = false;
    private float timeWalking = 0;

    public override void Start()
    {
        base.Start();
        aiPath = GetComponent<AIPath>();
        ai = GetComponent<IAstarAI>();

        aiPath.maxSpeed = moveSpeed;
    }

    public virtual void Update()
    {
        if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath) && !isOnWanderingCooldown)
        {
            isOnWanderingCooldown = true;
            ai.destination = PickRandomPoint();
            ai.SearchPath();
            FindDestination();
        }

        if (ai.velocity.x >= 0.05f || ai.velocity.y >= 0.05f)
        {
            anim.SetBool("isMoving", true);
        }
        else
        {
            anim.SetBool("isMoving", false);
        }

        if (ai.velocity.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        } else if (ai.velocity.y > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    private void FindDestination()
    {
        StartCoroutine(FindDestinationRoutine());
    }

    IEnumerator FindDestinationRoutine()
    {
        timeWalking = 0;

        while (Vector2.Distance(transform.position, ai.destination) > 2f)
        {
            timeWalking += Time.deltaTime;

            if (timeWalking >= maxTimeWalking)
            {
                ai.destination = transform.position;
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(Random.Range(timeBetweenWanderingMin, timeBetweenWanderingMax));

        isOnWanderingCooldown = false;
    }

    Vector2 PickRandomPoint()
    {
        var point = Random.insideUnitSphere * wanderRadius;
        point += transform.position;
        if (Vector2.Distance(point, transform.position) < minWanderDistance)
        {
            point = PickRandomPoint();
        }
        point.z = 0;

        //point += ai.position;
        return point;
    }
}
