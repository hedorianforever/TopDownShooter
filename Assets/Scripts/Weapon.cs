using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab = default;
    [SerializeField] Transform shotPoint = default;
    [SerializeField] float timeBetweenShots = 1f;

    public float projectileSpeed = 10f;
    public float projectileLifetime = 5f;

    private bool isOnCooldown = false;

    private void Update()
    {
        LookAtMouse();
        Shoot();
    }

    private void LookAtMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDirection = mousePos - transform.position;

        lookDirection.Normalize(); //not really necessary, just showing that it doesn't matter

        transform.up = lookDirection; //transform.up = basically the same as rotation.z

        transform.Rotate(new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z + 90)); //add 90 to final rotation because the weapon is initially looking right;
    }

    private void Shoot()
    {
        if (Input.GetMouseButton(0))
        {
            if (!isOnCooldown)
            {
                StartCoroutine(ShootCoroutine());
            }
        }
    }

    IEnumerator ShootCoroutine()
    {
        isOnCooldown = true;

        GameObject projectile = Instantiate(projectilePrefab, shotPoint.position, transform.rotation) as GameObject;
        projectile.GetComponent<Projectile>().Init(this);

        yield return new WaitForSeconds(timeBetweenShots);

        isOnCooldown = false;
    }
}
