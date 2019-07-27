using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPortal : MonoBehaviour
{
    [SerializeField] Transform portalCenter = default;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(ClosePortalRoutine());
        }
    }

    IEnumerator ClosePortalRoutine()
    {
        player.GetComponent<Animator>().SetTrigger("teleportTrigger");

        float t = 0;

        Vector2 originalPos = player.transform.position;

        //brings player to the center of the portal
        while (t < 1)
        {
            player.transform.position = Vector2.Lerp(originalPos, portalCenter.position, t);
            t += Time.deltaTime;
            yield return null;
        }

        //makes player disappear
        player.SetActive(false);

        GetComponent<Animator>().SetTrigger("closeTrigger");
    }

    public IEnumerator GoToNextLevel()
    {
        yield return new WaitForSeconds(.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("COLLISION");
    }
}
