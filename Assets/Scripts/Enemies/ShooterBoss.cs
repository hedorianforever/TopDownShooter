using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class ShooterBoss : Enemy
{
    //use parent class' timebetweenattacks and attack damage for shooting
    [Header("Boss related variables")]
    [SerializeField] float accuracyOffset = 5f;
    [SerializeField] float projectileSpeed = 10f;
    //[Range(0, 100)] [SerializeField] int chanceToShootOnSight = 20;
    [SerializeField] float timeToWaitBeforeAttacking = 3f;
    [SerializeField] float timeShooting = 5f;

    [SerializeField] Transform machineGunTransform = default; //to look at player 
    [SerializeField] Transform machineGunShotPoint = default;
    [SerializeField] Transform shotgunTransform = default;
    [SerializeField] Transform[] shotgunShotPoints = default;
    [SerializeField] GameObject projectilePrefab = default;

    [SerializeField] AudioClip shootSound1 = default;
    [SerializeField] AudioClip shootSound2 = default;

    [SerializeField] float stopDistance = 3f;
    [SerializeField] float leapAttackSpeed = 4f;

    private AIPath aiPath;

    private bool isAttacking = false;
    private bool isMoving = true;
    private bool isUsingMachineGun = true;
    private bool isEnraged = false;

    private int maxHealth;

    //private Rigidbody2D rb;

    public override void Start()
    {
        base.Start();

        maxHealth = health;

        Physics2D.queriesStartInColliders = false;
        //rb = GetComponent<Rigidbody2D>();
        aiPath = GetComponent<AIPath>();

        aiPath.maxSpeed = moveSpeed;
        aiPath.endReachedDistance = stopDistance;

        //should wait a few seconds to change this
        aiPath.canMove = true;

        if (playerTransform != null && GetComponent<AIDestinationSetter>().target == null)
        {
            GetComponent<AIDestinationSetter>().target = playerTransform;
        }

        StartCoroutine(BossRoutine());
    }

    IEnumerator BossRoutine()
    {
        while (playerTransform != null)
        {
            yield return StartCoroutine(AttackStage());
            yield return StartCoroutine(WaitStage());

            if (health <= maxHealth / 2 && !isEnraged)
            {
                //turns red
                Debug.Log(GetComponent<SpriteRenderer>().color);
                GetComponent<SpriteRenderer>().color = new Color32(255, 67, 67, 255);
                Debug.Log(GetComponent<SpriteRenderer>().color);
                Debug.Log(name + " IS FURIOUS!");
                timeBetweenAttacks *= .6f;
                timeToWaitBeforeAttacking = .8f;
                aiPath.maxSpeed *= 1.2f;
                isEnraged = true;
            }
        }

    }

    IEnumerator WaitStage()
    {
        anim.SetBool("isMoving", false);
        aiPath.canMove = false;
        yield return new WaitForSeconds(timeToWaitBeforeAttacking);
        ChangeWeapon();
    }

    IEnumerator AttackStage()
    {
        aiPath.canMove = true;
        anim.SetBool("isMoving", true);
        float t = 0;
        while (t < timeShooting)
        {
            Shoot();
            FacePlayer();
            yield return null;
            t += Time.deltaTime;
            //if is enraged, stop this attack stage
            if (health <= maxHealth / 2 && !isEnraged)
            {
                break;
            }
        }
    }

    //private void Update()
    //{
    //    if (playerTransform == null) { return; }

    //    if ((Mathf.Abs(aiPath.velocity.y) >= .1f || Mathf.Abs(aiPath.velocity.x) >= .1f))
    //    {
    //        anim.SetBool("isMoving", true);

    //    }
    //    else
    //    {
    //        anim.SetBool("isMoving", false);
    //    }

    //    FacePlayer();
    //}

    private void LookAtPlayer()
    {
        var lookDirection = playerTransform.position - machineGunTransform.position;
        FacePlayer();
        machineGunTransform.right = lookDirection;
        shotgunTransform.right = lookDirection;

    }

    private void Shoot()
    {
        if (playerTransform == null) { return; }

        Vector2 directionToPlayer = playerTransform.position - transform.position;

        //checks if player is on sight
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, directionToPlayer, 100);
        if (!hitInfo) { return; }

        //Player is on enemy's line of sight
        if (hitInfo.collider.gameObject.tag == "Player")
        {
            Debug.DrawLine(transform.position, hitInfo.point, Color.red);
            //TODO: maybe should show this line as a preview for the player to dodge
            LookAtPlayer();
            if (!isAttacking)
            {
                isAttacking = true;
                StartCoroutine(ShootRoutine());
            }
        }
        else
        {
            Debug.DrawLine(transform.position, hitInfo.point, Color.green);
        }
    }

    IEnumerator ShootRoutine()
    {
        if (isUsingMachineGun)
        {
            ShootMachineGun();
            yield return new WaitForSeconds(timeBetweenAttacks);
        }
        else
        {
            ShootShotgun();
            yield return new WaitForSeconds(timeBetweenAttacks *  3);
        }

        isAttacking = false;
    }

    void ChangeWeapon()
    {
        isUsingMachineGun = !isUsingMachineGun;

        machineGunTransform.gameObject.SetActive(isUsingMachineGun);
        shotgunTransform.gameObject.SetActive(!isUsingMachineGun);
    }

    void ShootMachineGun()
    {
        AudioManager.Instance.PlayClip(shootSound1, .5f);
        GameObject projectile = Instantiate(projectilePrefab, machineGunShotPoint.position, machineGunTransform.rotation) as GameObject;

        float randomRotation = Random.Range(-accuracyOffset, accuracyOffset);

        projectile.transform.Rotate(new Vector3(
            projectile.transform.rotation.x,
            projectile.transform.rotation.y,
            projectile.transform.rotation.z + randomRotation)
        );

        if (projectile != null)
        {
            projectile.GetComponent<EnemyProjectile>().Init(attackDamage, projectileSpeed);
        }
    }

    void ShootShotgun()
    {
        AudioManager.Instance.PlayClip(shootSound2, .6f);
        foreach (Transform shotPoint in shotgunShotPoints)
        {
            GameObject projectile = Instantiate(projectilePrefab, shotPoint.position, shotPoint.rotation) as GameObject;

            if (projectile != null)
            {
                projectile.GetComponent<EnemyProjectile>().Init(attackDamage, projectileSpeed);
            }
        }
    }

    //returns a random point from which the player is in the line of sight of the boss
    //Vector2 PickRandomPoint()
    //{
    //    var point = Random.insideUnitSphere * moveRadius;
    //    point += transform.position;

    //    //if point is inside an obstacle, pick another point
    //    Collider2D collider = Physics2D.OverlapCircle(point, .1f, 1 << LayerMask.NameToLayer("Obstacle"));
    //    if (collider != null)
    //    {
    //        point = PickRandomPoint();
    //    }

    //    //if can't see player from the new point, don't move there
    //    Vector2 directionToPlayer = playerTransform.position - transform.position;
    //    RaycastHit2D hitInfo = Physics2D.Raycast(point, directionToPlayer, 100);

    //    if (!hitInfo)
    //    {
    //        Debug.LogError("ERROR: no hit info inside PickRandomPoint");
    //        Debug.Break();
    //    }
    //    //can't see the player from the chosen point
    //    if (hitInfo.collider.gameObject.tag != "Player")
    //    {
    //        point = PickRandomPoint();
    //    }

    //    point.z = 0;

    //    return point;
    //}
}
