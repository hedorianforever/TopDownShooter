using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameObject playerPortal = default;

    //private List<Enemy> enemies = new List<Enemy>();
    private int enemyCount = 0;
    private Player player;
    private int playerHealth = 0;

    private void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        if (player != null)
        {
            if (playerHealth != 0)
            {
                //player's health = playerHealth
                player.SetHealth(playerHealth);
            } else
            {
                player.SetHealth((int)player.GetMaxHealth());
            }
        }
    }

    public void IncreaseEnemyCount()
    {
        enemyCount++;
    }

    public void SetEnemyCount(int n)
    {
        enemyCount = n;
    }

    public void DecreaseEnemyCount(Vector3 lastEnemyPosition)
    {
        enemyCount--;
        if (enemyCount == 0) {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            playerHealth = (int)player.GetCurrentHealth();
            Instantiate(playerPortal, lastEnemyPosition, Quaternion.identity);
        }
    }
}
