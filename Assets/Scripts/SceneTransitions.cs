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

    public void LoadLoseScene()
    {
        StartCoroutine(LoseSceneRoutine());
    }

    public void LoadWinScene()
    {
        StartCoroutine(WinSceneRoutine());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator LoseSceneRoutine()
    {
        yield return new WaitForSeconds(2f);

        anim.SetTrigger("endTrigger");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Lose Scene");
    }

    IEnumerator WinSceneRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        anim.SetTrigger("endTrigger");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Win Scene");
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
