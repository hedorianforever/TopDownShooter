using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWeapon : Weapon
{
    [Header("LaserWeapon specific variables")]
    [SerializeField] LineRenderer lineRenderer = default;
    [SerializeField] LayerMask layerMask = default;
    [SerializeField] protected GameObject impactFX = default;

    public override IEnumerator ShootCoroutine()
    {
        float randomRotation = Random.Range(-accuracyOffset, accuracyOffset);

        shotPoint.Rotate(new Vector3(0, 0, randomRotation));

        RaycastHit2D hitInfo = Physics2D.Raycast(shotPoint.position, shotPoint.right, 1000, layerMask);

        if (hitInfo)
        {
            Enemy enemy = hitInfo.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Instantiate(impactFX, hitInfo.point, Quaternion.identity);

            lineRenderer.SetPosition(0, shotPoint.position);
            lineRenderer.SetPosition(1, hitInfo.point);
        } else
        {
            lineRenderer.SetPosition(0, shotPoint.position);
            lineRenderer.SetPosition(1, shotPoint.position + shotPoint.right * 100);

        }

        shotPoint.Rotate(new Vector3(0, 0, -randomRotation));


        lineRenderer.enabled = true;

        yield return new WaitForSeconds(.05f);

        //shotPoint.rotation = Quaternion.identity;
        lineRenderer.enabled = false;

        yield return null;
    }
}
