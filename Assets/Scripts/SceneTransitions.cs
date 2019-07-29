using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitions : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(Transition(sceneName));
    }

    public void LoadNextScene()
    {
        StartCoroutine(NextSceneTransition());
    }

    IEnumerator Transition(string sceneName)
    {
        anim.SetTrigger("endTrigger");

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator NextSceneTransition()
    {
        anim.SetTrigger("endTrigger");

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

}
