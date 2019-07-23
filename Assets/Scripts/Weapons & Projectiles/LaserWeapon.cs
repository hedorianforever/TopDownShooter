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

        RaycastHit2D[] hitsInfo = Physics2D.RaycastAll(shotPoint.position, shotPoint.right, 1000, layerMask);

        foreach (RaycastHit2D hit in hitsInfo)
        {
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Instantiate(impactFX, hit.point, Quaternion.identity);
            }
            if (hit.transform.tag == "Obstacle")
            {
                Instantiate(impactFX, hit.point, Quaternion.identity);
                lineRenderer.SetPosition(0, shotPoint.position);
                lineRenderer.SetPosition(1, hit.point);
                break;
            }
        }

        //rotate shot point to original position
        shotPoint.Rotate(new Vector3(0, 0, -randomRotation));

        lineRenderer.enabled = true;

        yield return new WaitForSeconds(.05f);

        lineRenderer.enabled = false;

        yield return null;

    }

}
