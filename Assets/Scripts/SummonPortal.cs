using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonPortal : MonoBehaviour
{
    [SerializeField] GameObject enemyToSummon = default;
    [SerializeField] Transform summonSpot = default;

    [SerializeField] AudioClip openPortalSound = default;
    [SerializeField] AudioClip closePortalSound = default;

    private void Start()
    {
        AudioManager.Instance.PlayClip(openPortalSound, 1, false);
    }


    public IEnumerator Summon()
    {
        GameObject summonedEnemy = Instantiate(enemyToSummon, summonSpot.position, Quaternion.identity);
        summonedEnemy.GetComponent<Enemy>().enabled = false;

        yield return new WaitForSeconds(1.4f);

        summonedEnemy.GetComponent<Enemy>().enabled = true;
        Destroy(gameObject);

    }

    public void ClosePortal()
    {
        AudioManager.Instance.PlayClip(closePortalSound, 1, false);
    }
}
