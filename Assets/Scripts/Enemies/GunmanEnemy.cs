using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunmanEnemy : WandererEnemy
{
    //use timebetweenattacks and attack damage for shooting
    [SerializeField] protected float shootingRange = 20f;
    [SerializeField] protected float accuracyOffset = 5f;
    [SerializeField] protected float projectileLifetime = 2f;
    [SerializeField] protected float projectileSpeed = 10f;
    [Range(0, 1)]
    [Tooltip("Chance to shoot player while on sight")]
    [SerializeField] protected float chanceToShoot = .2f;
    //[Range(0, 100)] [SerializeField] int chanceToShootOnSight = 20;

    [SerializeField] protected Transform gunTransform = default; //to look at player 
    [SerializeField] private Transform shotPoint = default;
    [SerializeField] protected GameObject projectilePrefab = default;


    protected bool isShooting = false;


    public override void Start()
    {
        base.Start();
        Physics2D.queriesStartInColliders = false;
    }

    public override void Update()
    {
        base.Update();

        Shoot();
    }

    private void Shoot()
    {
        if (playerTransform == null) { return; }

        Vector2 directionToPlayer = playerTransform.position - transform.position;

        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, directionToPlayer, shootingRange);

        if (!hitInfo) { return; }
        //Player is on enemy's line of sight
        if (hitInfo.collider.gameObject.tag == "Player")
        {
            Debug.DrawLine(transform.position, hitInfo.point, Color.red);
            //TODO: maybe should show this line as a preview for the player to dodge
            LookAtPlayer();
            if (!isShooting && hasNoticedPlayer && Random.value < chanceToShoot)
            {
                isShooting = true;
                StartCoroutine(ShootRoutine());
            }
        }
        else
        {
            Debug.DrawLine(transform.position, hitInfo.point, Color.green);
        }
    }

    private void LookAtPlayer()
    {
        var lookDirection = playerTransform.position - gunTransform.position;
        FacePlayer();
        gunTransform.right = lookDirection;
    }

    public virtual IEnumerator ShootRoutine()
    {
        GameObject projectile = Instantiate(projectilePrefab, shotPoint.position, gunTransform.rotation) as GameObject;

        float randomRotation = Random.Range(-accuracyOffset, accuracyOffset);

        projectile.transform.Rotate(new Vector3(
            projectile.transform.rotation.x,
            projectile.transform.rotation.y,
            projectile.transform.rotation.z + randomRotation)
        );

        if (projectile != null)
        {
            projectile.GetComponent<EnemyProjectile>().Init(attackDamage, projectileSpeed, projectileLifetime);
        }

        yield return new WaitForSeconds(timeBetweenAttacks);

        isShooting = false;
    }
}