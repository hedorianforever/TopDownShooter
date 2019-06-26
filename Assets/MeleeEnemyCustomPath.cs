//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Pathfinding;

//public class MeleeEnemyCustomPath : Enemy
//{
//    [SerializeField] float stopDistance;

//    public float nextWaypointDistance = 3f;

//    Path path;
//    int currentWaypoint = 0;
//    bool reachedEndOfPath = false;
//    Seeker seeker;
//    Rigidbody2D rb;

//    public override void Start()
//    {
//        base.Start();
//        seeker = GetComponent<Seeker>();
//        rb = GetComponent<Rigidbody2D>();

//        //generate path at .5f rate
//        InvokeRepeating("UpdatePath", 0f, .2f);
//    }

//    void UpdatePath()
//    {
//        if (seeker.IsDone())
//        {
//            seeker.StartPath(rb.position, playerTransform.position, OnPathComplete);
//        }
//    }

//    void OnPathComplete(Path p)
//    {
//        if (!p.error)
//        {
//            //current path = generated path
//            path = p;
//            currentWaypoint = 0;
//        }
//    }

//    void FixedUpdate()
//    {
//        if (path == null) { return; }

//        //greater than the total amounts of waypoints along the current path
//        if (currentWaypoint >= path.vectorPath.Count)
//        {
//            reachedEndOfPath = true;
//            return;
//        }
//        else
//        {
//            reachedEndOfPath = false;
//        }

//        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
//        Vector2 force = direction * moveSpeed * Time.deltaTime;

//        rb.AddForce(force);

//        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
//        if (distance < nextWaypointDistance)
//        {
//            currentWaypoint++;
//        }

//        if (force.x >= 0.01f)
//        {
//            transform.localScale = new Vector3(-1f, 1f, 1f);
//        } else if (force.x <= -0.01f)
//        {
//            transform.localScale = new Vector3(1f, 1f, 1f);
//        }

        
//    }
//}
