using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPortal : MonoBehaviour
{
    [SerializeField] Transform portalCenter = default;

    [SerializeField] string targetSceneName = null;

    [SerializeField] AudioClip openPortalSound = default;
    [SerializeField] AudioClip closePortalSound = default;

    private GameObject player;
    private SceneTransitions sceneTransition;

    private void Start()
    {
        AudioManager.Instance.PlayClip(openPortalSound, 1, false);
        player = GameObject.FindGameObjectWithTag("Player");
        sceneTransition = GameObject.FindGameObjectWithTag("SceneTransitions").GetComponent<SceneTransitions>();
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

    public void ClosePortal()
    {
        AudioManager.Instance.PlayClip(closePortalSound, 1, false);
    }

    //public IEnumerator GoToNextLevel()
    //{
    //    yield return new WaitForSeconds(.5f);

    //    if (targetSceneName != null)
    //    {
    //        sceneTransition.LoadScene(targetSceneName);
    //    } else
    //    {
    //        sceneTransition.LoadNextScene();
    //    }
    //}

    public void GoToNextLevel()
    {
        if (targetSceneName != "")
        {
            sceneTransition.LoadScene(targetSceneName);
        }
        else
        {
            sceneTransition.LoadNextScene();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("COLLISION");
    }
}
